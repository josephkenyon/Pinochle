import React, { Component } from 'react';
import './App.css'

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);

        this.state = {
            gameName: '',
            playerName: '',
            playerState: null,
            loading: true
        };
    }

    componentDidMount() {
        this.setState({ ...this.state, loading: false})
    }

    renderEntry(state) {
        return (
            <div className="vertical-div">
                <input
                    className="input"
                    type="text"
                    value={this.state.gameName}
                    placeholder="Enter a game name"
                    onChange={event => this.setGameName(event)}
                />

                <input
                    className="input"
                    type="text"
                    value={this.state.playerName}
                    placeholder="Enter your player name"
                    onChange={event => this.setPlayerName(event)}
                />

                <button
                    className="button"
                    value="test"
                >
                Join Game
                </button>
            </div>
        );
    }

    renderGame(forecasts) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Temp. (C)</th>
                        <th>Temp. (F)</th>
                        <th>Summary</th>
                    </tr>
                </thead>
                <tbody>
                    
                </tbody>
            </table>
        );
    }

    render() {
        let contents

        if (!this.state.playerState) {
            contents = this.state.loading
                ? <p><em>Loading...</em></p>
                : this.renderEntry(this.state);
        } else {
            contents = this.state.loading
                ? <p><em>Loading...</em></p>
                : this.renderGame(this.state);
        }

        return (
            <div>
                <h1 id="tabelLabel">Pinochle</h1>
                {contents}
            </div>
        );
    }

    setGameName(event) {
        const newState = this.state
        newState.gameName = event.target.value
        this.setState({ ...this.state, gameName: event.target.value })
        console.log(this.state)
    }

    setPlayerName(event) {
        const newState = this.state
        newState.playerName = event.target.value
        this.setState(newState)
        console.log(this.state)
    }

    async populateWeatherData() {
        //const requestOptions = {
        //    method: 'GET',
        //    headers: { 'gameName': 'test', 'playerName': '1' }
        //}; 
        const response = await fetch('game');
        const data = await response.json();
        
        this.setState({ playerState: data, loading: false });
    }
}
