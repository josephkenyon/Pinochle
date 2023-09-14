import '../../../App.css'
import React from 'react';
import ConnectionService from '../../../services/connectionService';
import { Dropdown, DropdownButton } from 'react-bootstrap';

export default function TrumpSelectionBox() {
    const declareTrumpSuit = async (trumpSuit) => {
        await ConnectionService.getConnection().invoke("DeclareTrump", trumpSuit)
    }

    return (
        <div className="vertical-div">
            <DropdownButton
                className="trump-dropdown"
                variant="secondary"
                onSelect={(eventKey, _) => declareTrumpSuit(+eventKey)}
                title="Declare Trump">

                <Dropdown.Item as="button" eventKey="0">Spades</Dropdown.Item>
                <Dropdown.Item as="button" eventKey="1">Hearts</Dropdown.Item>
                <Dropdown.Item as="button" eventKey="2">Clubs</Dropdown.Item>
                <Dropdown.Item as="button" eventKey="3">Diamonds</Dropdown.Item>
            </DropdownButton>
        </div>
    )
}
