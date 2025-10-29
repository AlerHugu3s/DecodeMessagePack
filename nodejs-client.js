const net = require('net');
const http = require('http');
const https = require('https');
const FormData = require('form-data');
const fs = require('fs');

class MessagePackTcpClient {
    constructor(host = 'localhost', port = 8888) {
        this.host = host;
        this.port = port;
    }

    async decodeFromBytes(messagePackData) {
        return new Promise((resolve, reject) => {
            const client = new net.Socket();

            client.connect(this.port, this.host, () => {
                console.log('连接到TCP服务:', `${this.host}:${this.port}`);

                // 发送数据长度（4字节）
                const lengthBuffer = Buffer.alloc(4);
                lengthBuffer.writeInt32BE(messagePackData.length, 0);
                client.write(lengthBuffer);

                // 发送MessagePack数据
                client.write(messagePackData);
            });

            client.on('data', (data) => {
                try {
                    // 读取响应长度
                    const responseLength = data.readInt32BE(0);
                    const responseData = data.slice(4, 4 + responseLength);
                    const responseJson = responseData.toString('utf8');
                    const response = JSON.parse(responseJson);

                    client.destroy();
                    resolve(response);
                } catch (error) {
                    client.destroy();
                    reject(error);
                }
            });

            client.on('error', (error) => {
                client.destroy();
                reject(error);
            });

            client.on('close', () => {
                console.log('TCP连接已关闭');
            });
        });
    }
}

class MessagePackHttpClient {
    constructor(baseUrl = 'http://localhost:5000') {
        this.baseUrl = baseUrl;
    }

    /**
     * 通过HTTP POST发送MessagePack文件并接收jsonObject
     * @param {string|Buffer} filePathOrBuffer - 文件路径或MessagePack缓冲区
     * @returns {Promise<Object>} 返回包含jsonObject的响应
     */
    async decodeFromFile(filePathOrBuffer) {
        return new Promise((resolve, reject) => {
            const FormData = require('form-data');
            const form = new FormData();

            if (Buffer.isBuffer(filePathOrBuffer)) {
                form.append('file', filePathOrBuffer, {
                    filename: 'messagepack.bin',
                    contentType: 'application/octet-stream'
                });
            } else {
                form.append('file', fs.createReadStream(filePathOrBuffer));
            }

            const url = new URL(`${this.baseUrl}/api/MessagePack/decode`);
            const options = {
                hostname: url.hostname,
                port: url.port || (url.protocol === 'https:' ? 443 : 80),
                path: url.pathname,
                method: 'POST',
                headers: form.getHeaders()
            };

            const req = (url.protocol === 'https:' ? https : http).request(options, (res) => {
                let data = '';

                res.on('data', (chunk) => {
                    data += chunk;
                });

                res.on('end', () => {
                    try {
                        const response = JSON.parse(data);
                        
                        // jsonObject已经是JavaScript对象，可以直接使用
                        if (response.success && response.jsonObject) {
                            console.log('接收到jsonObject:', JSON.stringify(response.jsonObject, null, 2));
                            // jsonObject可以直接作为JavaScript对象使用
                            resolve({
                                ...response,
                                jsonObject: response.jsonObject  // 已经是JavaScript对象
                            });
                        } else {
                            resolve(response);
                        }
                    } catch (error) {
                        reject(new Error(`解析响应失败: ${error.message}`));
                    }
                });
            });

            req.on('error', (error) => {
                reject(error);
            });

            form.pipe(req);
        });
    }

    /**
     * 通过HTTP POST发送MessagePack字节数据并接收jsonObject
     * @param {Buffer} messagePackData - MessagePack字节数据
     * @returns {Promise<Object>} 返回包含jsonObject的响应
     */
    async decodeFromBytes(messagePackData) {
        return new Promise((resolve, reject) => {
            const url = new URL(`${this.baseUrl}/api/MessagePack/decode-from-bytes`);
            const options = {
                hostname: url.hostname,
                port: url.port || (url.protocol === 'https:' ? 443 : 80),
                path: url.pathname,
                method: 'POST',
                headers: {
                    'Content-Type': 'application/octet-stream',
                    'Content-Length': messagePackData.length
                }
            };

            const req = (url.protocol === 'https:' ? https : http).request(options, (res) => {
                let data = '';

                res.on('data', (chunk) => {
                    data += chunk;
                });

                res.on('end', () => {
                    try {
                        const response = JSON.parse(data);
                        
                        // jsonObject已经是JavaScript对象，可以直接使用
                        if (response.success && response.jsonObject) {
                            console.log('接收到jsonObject:', JSON.stringify(response.jsonObject, null, 2));
                            // jsonObject可以直接作为JavaScript对象使用
                            resolve({
                                ...response,
                                jsonObject: response.jsonObject  // 已经是JavaScript对象
                            });
                        } else {
                            resolve(response);
                        }
                    } catch (error) {
                        reject(new Error(`解析响应失败: ${error.message}`));
                    }
                });
            });

            req.on('error', (error) => {
                reject(error);
            });

            req.write(messagePackData);
            req.end();
        });
    }
}

// 使用示例
async function example() {
    const httpClient = new MessagePackHttpClient('http://localhost:5000');
    
    try {
        // 示例1: 从文件解码
        const fileResult = await httpClient.decodeFromFile('./example.msgpack');
        console.log('文件解码结果:');
        console.log('jsonObject:', fileResult.jsonObject);  // 直接使用JavaScript对象
        console.log('jsonData:', fileResult.jsonData);     // JSON字符串
        
        // 示例2: 从字节数据解码
        const buffer = fs.readFileSync('./example.msgpack');
        const bytesResult = await httpClient.decodeFromBytes(buffer);
        console.log('字节解码结果:');
        console.log('jsonObject:', bytesResult.jsonObject);  // 直接使用JavaScript对象
        
        // 示例3: 直接操作jsonObject
        if (bytesResult.jsonObject) {
            // jsonObject已经是JavaScript对象，可以直接访问属性
            console.log('访问jsonObject属性:', bytesResult.jsonObject);
            
            // 如果是对象，可以遍历
            if (typeof bytesResult.jsonObject === 'object' && !Array.isArray(bytesResult.jsonObject)) {
                Object.keys(bytesResult.jsonObject).forEach(key => {
                    console.log(`${key}:`, bytesResult.jsonObject[key]);
                });
            }
        }
    } catch (error) {
        console.error('错误:', error.message);
    }
}

module.exports = { MessagePackTcpClient, MessagePackHttpClient };
