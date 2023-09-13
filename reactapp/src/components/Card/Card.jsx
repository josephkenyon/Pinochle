import '../../App.css'
import React, { Component } from 'react';

const suits = ["♠︎", "♥︎", "♣︎", "♦︎"]
const ranks = ["A", "10", "K", "Q", "J", "9"]

export default class Card extends Component {
    constructor(props) {
        super(props);

        this.state = {
            suit: suits[props.suit],
            rank: ranks[props.rank],
            zIndex: props.zIndex,
            small: props.small || false,
            selected: props.selected || false,
            onClick: props.onClick
        };
    }

    render() {
        const colorClass = (this.state.suit == "♣︎" || this.state.suit == "♠︎") ? "card-black" : "card-red"

        const smallClassClause = this.state.small ? "small-card" : "large-card"
        const leftMarginClause = (this.state.zIndex > 0) ? (this.state.small ? "card-left-margin-small" : "card-left-margin-large") : ""
        const topMarginClause = this.state.selected ? (this.state.small ? "card-top-margin-small" : "card-top-margin-large") : ""

        return (
            <div className={["card", smallClassClause, colorClass, leftMarginClause, topMarginClause].join(" ")} style={{zIndex: this.state.zIndex}} onClick={() => this.state.onClick()}>
                    <div className="card-tl" onClick={() => this.state.onClick()}>
                        <div className={(this.state.small ? "card-value-small" : "card-value-large")} onClick={() => this.state.onClick()}>{this.state.rank}</div>
                        <div className={(this.state.small ? "card-suit-small" : "card-value-large")} onClick={() => this.state.onClick()}>{this.state.suit}</div>
                    </div>
                    <div className="card-br">
                        <div className={(this.state.small ? "card-value-small" : "card-value-large")} onClick={() => this.state.onClick()}>{this.state.rank}</div>
                        <div className={(this.state.small ? "card-suit-small" : "card-value-large")} onClick={() => this.state.onClick()}>{this.state.suit}</div>
                    </div>
                </div>
        )
    }
}
