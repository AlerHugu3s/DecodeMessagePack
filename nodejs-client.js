const net = require('net');

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

module.exports = MessagePackTcpClient;
