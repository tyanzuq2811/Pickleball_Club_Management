import * as signalR from '@microsoft/signalr';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5078';

class SignalRService {
    constructor() {
        this.connection = null;
        this.isConnected = false;
    }

    async startConnection(token) {
        if (this.connection) {
            await this.stopConnection();
        }

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${API_URL}/hubs/scoreboard`, {
                accessTokenFactory: () => token,
                skipNegotiation: false,
                transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.connection.onreconnecting(() => {
            console.log('[SignalR] Reconnecting...');
            this.isConnected = false;
        });

        this.connection.onreconnected(() => {
            console.log('[SignalR] Reconnected');
            this.isConnected = true;
        });

        this.connection.onclose(() => {
            console.log('[SignalR] Connection closed');
            this.isConnected = false;
        });

        try {
            await this.connection.start();
            this.isConnected = true;
            console.log('[SignalR] Connected successfully');
        } catch (error) {
            console.error('[SignalR] Connection failed:', error);
            this.isConnected = false;
            throw error;
        }
    }

    async stopConnection() {
        if (this.connection) {
            try {
                await this.connection.stop();
                this.isConnected = false;
                console.log('[SignalR] Connection stopped');
            } catch (error) {
                console.error('[SignalR] Error stopping connection:', error);
            }
            this.connection = null;
        }
    }

    // Subscribe to score updates
    onScoreUpdated(callback) {
        if (this.connection) {
            this.connection.on('ScoreUpdated', callback);
        }
    }

    // Subscribe to match finished
    onMatchFinished(callback) {
        if (this.connection) {
            this.connection.on('MatchFinished', callback);
        }
    }

    // Subscribe to booking updates
    onBookingCreated(callback) {
        if (this.connection) {
            this.connection.on('BookingCreated', callback);
        }
    }

    onBookingCancelled(callback) {
        if (this.connection) {
            this.connection.on('BookingCancelled', callback);
        }
    }

    // Subscribe to notifications
    onNewNotification(callback) {
        if (this.connection) {
            this.connection.on('ReceiveNotification', callback);
        }
    }

    // Remove event listeners
    off(eventName) {
        if (this.connection) {
            this.connection.off(eventName);
        }
    }

    // Check if connected
    get connected() {
        return this.isConnected && this.connection?.state === signalR.HubConnectionState.Connected;
    }
}

export default new SignalRService();
