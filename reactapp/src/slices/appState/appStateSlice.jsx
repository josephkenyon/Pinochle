import { createSlice } from '@reduxjs/toolkit'

const initialState = {
    connection: null,
    gameName: '',
    playerName: '',
    errorMessage: '',
    loading: false
}

export const appStateSlice = createSlice({
  name: 'appState',
  initialState,
  reducers: {
    setConnection: (state, action) => {
        state.connection = action.payload
    },
    setGameName: (state, action) => {
        state.gameName = action.payload
    },
    setPlayerName: (state, action) => {
        state.playerName = action.payload
    },
    setErrorMessage: (state, action) => {
        state.errorMessage = action.payload
    },
    setLoading: (state, action) => {
        state.loading = action.payload
    }
  },
})

export const { setConnection, setGameName, setPlayerName, setErrorMessage, setLoading } = appStateSlice.actions

export default appStateSlice.reducer