import { createSlice } from '@reduxjs/toolkit'

const initialState = {
    lastBid: -1,
    currentBid: 15,
    hasState: false,
    isReady: false,
    showReady: false,
    showLastBid: false,
    showBiddingBox: false,
    showTrumpSelection: false,
    showSwapPlayerIndex: false,
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
        state.lastrickStatetBid = action.payload
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

        newHand.forEach(card => {
            if (card.id == action.payload) {
                card.selected = !card.selected;
            } else {
                card.selected = false;
            }
        });
    
        state.hand = newHand
    },
  },
})

export const { setLastBid, setCurrentBid, setHasState, setIsReady, setShowReady, setShowLastBid, setShowBiddingBox, setShowTrumpSelection, setShowSwapPlayerIndex, setHand,
    setDisplayedCards, setTrickState, setAllyState, setLeftOpponentState, setRightOpponentState, selectCard } = playerStateSlice.actions

export default playerStateSlice.reducer