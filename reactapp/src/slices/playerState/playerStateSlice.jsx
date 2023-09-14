import { createSlice } from '@reduxjs/toolkit'

const initialState = {
    teamOneName: "",
    teamTwoName: "",
    teamOneScoreLog: [],
    teamTwoScoreLog: [],
    roundBidResults: [],
    lastBid: -1,
    hasState: false,
    isReady: false,
    showReady: false,
    showLastBid: false,
    showBiddingBox: false,
    showPlayButton: false,
    showCollectButton: false,
    showTrumpSelection: false,
    showSwapPlayerIndex: false,
    hasSelectedCard: false,
    selectedCardId: -1,
    hand: [],
    displayedCards: [],
    trickState: {},
    allyState: {},
    leftOpponentState: {},
    rightOpponentState: {}
}

export const playerStateSlice = createSlice({
  name: 'playerState',
  initialState,
  reducers: {
    setTeamOneName: (state, action) => {
        state.teamOneName = action.payload
    },
    setTeamTwoName: (state, action) => {
        state.teamTwoName = action.payload
    },
    setTeamOneScoreLog: (state, action) => {
        state.teamOneScoreLog = action.payload
    },
    setTeamTwoScoreLog: (state, action) => {
        state.teamTwoScoreLog = action.payload
    },
    setRoundBidResults: (state, action) => {
        state.roundBidResults = action.payload
    },
    setLastBid: (state, action) => {
        state.lastBid = action.payload
    },
    setCurrentBid: (state, action) => {
        state.currentBid = action.payload
    },
    setHasState: (state, action) => {
        state.hasState = action.payload
    },
    setIsReady: (state, action) => {
        state.isReady = action.payload
    },
    setShowReady: (state, action) => {
        state.showReady = action.payload
    },
    setShowLastBid: (state, action) => {
        state.showLastBid = action.payload
    },
    setShowBiddingBox: (state, action) => {
        state.showBiddingBox = action.payload
    },
    setShowTrumpSelection: (state, action) => {
        state.showTrumpSelection = action.payload
    },
    setShowPlayButton: (state, action) => {
        state.showPlayButton = action.payload
    },
    setShowCollectButton: (state, action) => {
        state.showCollectButton = action.payload
    },
    setShowSwapPlayerIndex: (state, action) => {
        state.showSwapPlayerIndex = action.payload
    },
    setHand: (state, action) => {
        state.hand = action.payload
    },
    setDisplayedCards: (state, action) => {
        state.displayedCards = action.payload
    },
    setTrickState: (state, action) => {
        state.trickState = action.payload
    },
    setAllyState: (state, action) => {
        state.allyState = action.payload
    },
    setLeftOpponentState: (state, action) => {
        state.leftOpponentState = action.payload
    },
    setRightOpponentState: (state, action) => {
        state.rightOpponentState = action.payload
    },
    selectCard: (state, action) => {
        const newHand = state.hand

        state.selectedCardId = -1;
        state.hasSelectedCard = false;

        newHand.forEach(card => {
            if (card.id == action.payload) {
                card.selected = !card.selected;
                state.hasSelectedCard = card.selected;
                state.selectedCardId = card.selected ? card.id : -1
            } else {
                card.selected = false;
            }
        });
    
        state.hand = newHand
    },
  },
})

export const { setTeamOneName, setTeamTwoName, setTeamOneScoreLog, setTeamTwoScoreLog, setRoundBidResults, setLastBid, setCurrentBid, setHasState, setIsReady,
    setShowReady, setShowLastBid, setShowBiddingBox, setShowTrumpSelection, setShowSwapPlayerIndex, setHand, setDisplayedCards, setShowPlayButton, setShowCollectButton,
    setTrickState, setAllyState, setLeftOpponentState, setRightOpponentState, selectCard } = playerStateSlice.actions

export default playerStateSlice.reducer