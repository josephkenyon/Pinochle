export default class ConnectionService {
    static connection;

    static getConnection() {
        return this.connection
    }

    static setConnection(connection) {
        this.connection = connection
    }
}
