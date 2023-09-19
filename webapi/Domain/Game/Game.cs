﻿using webapi.Domain.Bidding;
using static webapi.Domain.Statics.Enums;

namespace webapi.Domain.Game
{
    public class Game : IGame
    {
        public string Name { get; set; }
        public Phase Phase { get; set; }
        public Suit TrumpSuit { get; set; }
        public int PlayerTurnIndex { get; set; }
        public int StartingPlayerTurnIndex { get; set; }
        public int TookBidTeamIndex { get; set; }
        public int CurrentBid { get; set; }
        public string TeamOneScoresString { get; set; }
        public string TeamTwoScoresString { get; set; }
        public string RoundBidResults { get; set; }
        public string TeamOneCardsTakenIds { get; set; }
        public string TeamTwoCardsTakenIds { get; set; }

        public Game()
        {
            Name = "Unknown";
            Phase = Phase.Initializing;
            TeamOneScoresString = "";
            TeamTwoScoresString = "";
            TeamOneCardsTakenIds = "";
            TeamTwoCardsTakenIds = "";
            RoundBidResults = "";
        }

        public Game(string gameName)
        {
            Name = gameName;
            Phase = Phase.Initializing;
            TeamOneScoresString = "";
            TeamTwoScoresString = "";
            TeamOneCardsTakenIds = "";
            TeamTwoCardsTakenIds = "";
            RoundBidResults = "";
            StartingPlayerTurnIndex = -1;
        }

        public int GetPlayerTurnIndex()
        {
            return PlayerTurnIndex;
        }

        public void SetPlayerTurnIndex(int index)
        {
            PlayerTurnIndex = index;
        }

        public int GetTookBidTeamIndex()
        {
            return TookBidTeamIndex;
        }

        public void SetTookBidTeamIndex(int index)
        {
            TookBidTeamIndex = index;
        }

        public int GetCurrentBid()
        {
            return CurrentBid;
        }

        public Phase GetPhase()
        {
            return Phase;
        }

        public Suit GetTrumpSuit()
        {
            return TrumpSuit;
        }

        public void SetTrumpSuit(Suit suit)
        {
            TrumpSuit = suit;
        }

        public void IncrementPhase()
        {
            var phaseInt = (int) Phase;

            Phase = (Phase) Enum.ToObject(typeof(Phase), phaseInt + 1);
        }

        public void SetCurrentBid(int bid)
        {
            CurrentBid = bid;
        }

        public void StartNewRound()
        {
            Phase = Phase.Bidding;

            StartingPlayerTurnIndex++;
            if (StartingPlayerTurnIndex > 3)
            {
                StartingPlayerTurnIndex = 0;
            }

            PlayerTurnIndex = StartingPlayerTurnIndex;
            CurrentBid = 14;
            TeamOneCardsTakenIds = "";
            TeamTwoCardsTakenIds = "";
        }

        public int IncrementAndGetStartingPlayerTurnIndex()
        {
            StartingPlayerTurnIndex++;

            if (StartingPlayerTurnIndex > 3)
            {
                StartingPlayerTurnIndex = 0;
            }

            return StartingPlayerTurnIndex;
        }

        public void AddScore(int teamIndex, int scoreIncrementValue)
        {
            var scoreArray = (teamIndex == 0 ? TeamOneScoresString : TeamTwoScoresString).Split(";").ToList();
            scoreArray.RemoveAll(string.IsNullOrEmpty);
            scoreArray.Add(scoreIncrementValue.ToString());

            if (teamIndex == 0)
            {
                TeamOneScoresString = string.Join(";", scoreArray);
            }
            else
            {
                TeamTwoScoresString = string.Join(";", scoreArray);
            }
        }

        public int GetLastMeld(int teamIndex)
        {
            var scoreArray = (teamIndex == 0 ? TeamOneScoresString : TeamTwoScoresString).Split(";").ToList();
            scoreArray.RemoveAll(string.IsNullOrEmpty);

            var lastMeld = scoreArray.Last();

            return int.Parse(lastMeld);
        }

        public void NullifyMeld(int teamIndex)
        {
            var scoreArray = (teamIndex == 0 ? TeamOneScoresString : TeamTwoScoresString).Split(";").ToList();
            scoreArray.RemoveAll(string.IsNullOrEmpty);

            scoreArray.RemoveAt(scoreArray.Count - 1);
            scoreArray.Add("0");

            if (teamIndex == 0)
            {
                TeamOneScoresString = string.Join(";", scoreArray);
            }
            else
            {
                TeamTwoScoresString = string.Join(";", scoreArray);
            }
        }

        public List<string> GetScoreLog(string teamName, int teamIndex)
        {

            var newList = new List<string>
            {
                teamName
            };

            newList.AddRange(teamIndex == 0 ? TeamOneScoresString.Split(";") : TeamTwoScoresString.Split(";"));

            return newList;
        }

        public int GetTotalScore(int teamIndex)
        {
            int scoreTotal = 0;

            foreach (var score in teamIndex == 0 ? TeamOneScoresString.Split(";") : TeamTwoScoresString.Split(";"))
            {
                var parsedScore = 0;
                try
                {
                    parsedScore = int.Parse(score);
                }
                catch (Exception) { }

                scoreTotal += parsedScore;
            }

            return scoreTotal;
        }

        public void AddRoundBidResult(Suit trumpSuit, int teamIndex, int bid)
        {
            var resultsList = RoundBidResults.Split(";").ToList();
            resultsList.RemoveAll(string.IsNullOrEmpty);

            var result = $"{trumpSuit}:{teamIndex}:{bid}";

            resultsList.Add(result);

            RoundBidResults = string.Join(";", resultsList);
        }

        public List<RoundBidResult> GetRoundBidResults()
        {
            var resultsList = RoundBidResults.Split(";").ToList();
            resultsList.RemoveAll(string.IsNullOrEmpty);

            var newList = new List<RoundBidResult>();

            foreach (var result in resultsList)
            {
                var resultArray = result.Split(":").ToList();
                _ = Enum.TryParse(resultArray[0], out Suit suit);

                newList.Add(new RoundBidResult(suit, int.Parse(resultArray[1]), int.Parse(resultArray[2])));
            }

            return newList;
        }

        public void AddCardIds(int teamIndex, List<int> cardIds)
        {
            var idsList = (teamIndex == 0 ? TeamOneCardsTakenIds : TeamTwoCardsTakenIds).Split(";").ToList();
            idsList.RemoveAll(string.IsNullOrEmpty);
            idsList.AddRange(cardIds.Select(id => id.ToString()));

            if (teamIndex == 0)
            {
                TeamOneCardsTakenIds = string.Join(";", idsList);
            }
            else
            {
                TeamTwoCardsTakenIds = string.Join(";", idsList);
            }
        }

        public List<int> GetCardIds(int teamIndex)
        {

            var newList = new List<int>();

            var ids = (teamIndex == 0 ? TeamOneCardsTakenIds.Split(";") : TeamTwoCardsTakenIds.Split(";")).ToList();
            ids.RemoveAll(string.IsNullOrEmpty);
            newList.AddRange(ids.Select(id => int.Parse(id)));

            return newList;
        }
    }
}
