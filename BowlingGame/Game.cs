using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace BowlingGame
{
	public class Game
	{
	    private int previousPins = 0;
	    private bool firstRollNotStrike = false;
	    private int bonusCurrent = 0;
	    private int bonusNext = 0;
	    private int currentScore = 0;
	    private int roundNumber = 0;
	    private bool endGame = false;
	    private int lastStrike = 0;

	    public void AddBonus(int addToCurrentBonus, int addToNextBonus)
	    {
	        bonusCurrent += addToCurrentBonus;
	        bonusNext += addToNextBonus;
	    }

		public void Roll(int pins)
		{
            if (endGame)
                throw new ArgumentException("End Game"); 
            if (pins > 10)
                throw new ArgumentException("More than ten");
		    if (firstRollNotStrike && pins + previousPins > 10)
                throw new ArgumentException("More than ten");

            currentScore += pins + pins * bonusCurrent;
            bonusCurrent = bonusNext;
		    bonusNext = 0;

            if (pins == 10)
            {
                AddBonus(1, 1);
                roundNumber++;
		        return;
		    }
		    
		    if (firstRollNotStrike)
		    {
		        if (pins + previousPins == 10)
		        {
		            AddBonus(1, 0);
		        }
		        firstRollNotStrike = false;
		        roundNumber++;
		    }
		    else
		    {
                if (pins < 10)
                {
                    firstRollNotStrike = true;
                    previousPins = pins;
                }
            }
		    if (roundNumber >= 10)
		    {
		        if (pins == 10)
		            lastStrike = 2;
		        endGame = true;
		        bonusNext = 0;
		        bonusCurrent = 0;
		        if (pins + previousPins == 10)
		        {
		            endGame = false;
		        }
		        if (lastStrike > 0)
		            endGame = false;
                lastStrike--;

            }
		    
		}

		public int GetScore()
		{
			return currentScore;
		}
	}


	[TestFixture]
	public class Game_should : ReportingTest<Game_should>
	{
		// ReSharper disable once UnusedMember.Global
		public static string Names = "8 Arapova Ovchinnikova"; // Ivanov Petrov

		[Test]
		public void HaveZeroScore_BeforeAnyRolls()
		{
			new Game()
				.GetScore()
				.Should().Be(0);
		}

        [TestCase(new int[] { 2 }, 2)]
        [TestCase(new int[] { 2, 3 }, 5)]
        public void GetRightScore_AfterRoll_WithoutSpireAnsStrike(int[] rolls, int score)
        {
            var game = new Game();
            foreach (var pins in rolls)
            {
                game.Roll(pins);
            }
            game.GetScore().Should().Be(score);
        }

	    [Test]
	    public void ThrowExcecpion_IfPinsCountMoreThanTen()
	    {
            var game = new Game();
	        Assert.Throws<ArgumentException>(() => game.Roll(11));
	    }

        [Test]
        public void ThrowExcecpion_IfSumPinsInRaundMoreThanTen()
        {
            var game = new Game();
            game.Roll(6);
            Assert.Throws<ArgumentException>(() => game.Roll(6));
        }

        [Test]
        public void ThrowExcecpion_WhenPlayerRollIfGameOver()
        {
            var game = new Game();
            for (int i = 0; i < 20; i++)
            {
                game.Roll(1);
            }
            Assert.Throws<ArgumentException>(() => game.Roll(6));
        }

        [TestCase(new int[] { 4, 6, 3 }, 16)]
        [TestCase(new int[] { 4, 6, 3, 2}, 18)]
        [TestCase(new int[] { 4, 6, 3, 7, 2 }, 27, TestName = "two spire")]
        public void GetRightScore_AfterSpire(int[] rolls, int score)
        {
            var game = new Game();
            foreach (var pins in rolls)
            {
                game.Roll(pins);
            }
            game.GetScore().Should().Be(score);
        }

        [TestCase(new int[] { 10, 6, 3 }, 28)]
        [TestCase(new int[] { 10, 10, 3, 1 }, 24 + 13 + 4)]
        [TestCase(new int[] { 10, 10, 10, 1, 3 }, 30 + 21 + 14 + 4)]
        public void GetRightScore_AfterStrike(int[] rolls, int score)
        {
            var game = new Game();
            foreach (var pins in rolls)
            {
                game.Roll(pins);
            }
            game.GetScore().Should().Be(score);
        }

        [Test]
	    public void HaveRightScore_AfterEndsOfGame_WithSpareInLastRound()
	    {
	        var rolls = new int[21];
	        for (int i = 0; i < 18; i++)
	            rolls[i] = 1;
	        rolls[18] = 1;
	        rolls[19] = 9;
	        rolls[20] = 5;
            var game = new Game();
            foreach (var pins in rolls)
            {
                game.Roll(pins);
            }
            game.GetScore().Should().Be(33);
        }
        [Test]
        public void HaveRightScore_AfterEndsOfGame_WithStrikeInLastRound()
        {
            var rolls = new int[21];
            for (int i = 0; i < 18; i++)
                rolls[i] = 1;
            rolls[18] = 10;
            rolls[19] = 2;
            rolls[20] = 3;
            var game = new Game();
            foreach (var pins in rolls)
            {
                game.Roll(pins);
            }
            game.GetScore().Should().Be(33);
        }
    }
}
