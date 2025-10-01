using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameController : IDisposable
{
    private GameField _gameField;
    private GameFieldView _gameFieldView;
    private GameConfiguration _gameConfiguration;
    private GameUI _gameUI;
    private LobbyUI _lobbyUI;
    private int _score;
    private int _bestScore;

    public GameController(GameField gameField, GameFieldView gameFieldView, GameConfiguration gameConfiguration, GameUI gameUI, LobbyUI lobbyUI)
    {
        _gameField = gameField;
        _gameFieldView = gameFieldView;
        _gameConfiguration = gameConfiguration;
        _gameUI = gameUI;
        _lobbyUI = lobbyUI;
        _bestScore = PlayerPrefs.GetInt("BestScore", 0);
        _lobbyUI.SetBestScore(_bestScore);
    }

    public void Dispose()
    {
        StopGame();
    }

    private void StopGame()
    {
        _gameField.Dispose();
        _gameFieldView.Dispose();
        _gameFieldView.gameObject.SetActive(false);

        if (_score > _bestScore)
        {
            _bestScore = _score;
            _lobbyUI.SetBestScore(_bestScore);
            PlayerPrefs.SetInt("BestScore", _bestScore);
        }
    }

    public void StartGame()
    {
        _gameFieldView.gameObject.SetActive(true);
        _gameFieldView.LinkModel(_gameField);
        for (int i = 0; i < _gameConfiguration.TileSpawnConfiguration.TilesOnInit; i++)
        {
            _gameField.FillRandomCell();
        }

        _score = 0;
        _gameField.GetScoreEvent += GetScoreHandler;
    }

    private void GetScoreHandler(int score)
    {
        _score += score;
        _gameUI.SetScore(_score);
    }
}
