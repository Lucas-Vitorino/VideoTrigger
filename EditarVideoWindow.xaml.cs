using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

using CorpoHumanoInterativo.Models;

namespace CorpoHumanoInterativo;

public partial class EditarVideoWindow : Window
{
    private readonly VideoAtalho _videoOriginal;

    private readonly HashSet<int> _pinosOcupados;

    private readonly HashSet<string> _nomesExistentes;

    private string? _caminhoNovoVideo;

    public VideoAtalho? VideoEditado { get; private set; }

    public string? CaminhoNovoVideo =>
        _caminhoNovoVideo;

    public EditarVideoWindow(
        VideoAtalho video,
        IEnumerable<VideoAtalho> videosExistentes)
    {
        InitializeComponent();

        _videoOriginal = video;

        _pinosOcupados =
            videosExistentes
                .Where(v => !ReferenceEquals(v, video))
                .Select(v => v.Pino)
                .ToHashSet();

        _nomesExistentes =
            videosExistentes
                .Where(v => !ReferenceEquals(v, video))
                .Select(v => v.Nome)
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase
                );

        CarregarDados();
    }

    private void CarregarDados()
    {
        NomeTextBox.Text =
            _videoOriginal.Nome;

        VideoTextBox.Text =
            _videoOriginal.Video;

        CarregarPinos();

        PinoComboBox.SelectedItem =
            _videoOriginal.Pino;
    }

    private void CarregarPinos()
    {
        PinoComboBox.Items.Clear();

        for (int pino = 2;
             pino <= 13;
             pino++)
        {
            if (!_pinosOcupados.Contains(pino))
            {
                PinoComboBox.Items.Add(pino);
            }
        }
    }

    private void BotaoSelecionarVideo_Click(
        object sender,
        RoutedEventArgs e)
    {
        OpenFileDialog janela = new()
        {
            Title = "Selecione o novo vídeo",

            Filter =
                "Vídeos MP4 (*.mp4)|*.mp4",

            Multiselect = false
        };

        bool? resultado =
            janela.ShowDialog();

        if (resultado != true)
            return;

        _caminhoNovoVideo =
            janela.FileName;

        VideoTextBox.Text =
            Path.GetFileName(
                janela.FileName
            );
    }

    private void BotaoSalvar_Click(
        object sender,
        RoutedEventArgs e)
    {
        string nome =
            NomeTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(nome))
        {
            MessageBox.Show(
                "Digite um nome para o botão.",
                "Editar vídeo",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        if (_nomesExistentes.Contains(nome))
        {
            MessageBox.Show(
                "Já existe outro botão com esse nome.",
                "Editar vídeo",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        if (PinoComboBox.SelectedItem == null)
        {
            MessageBox.Show(
                "Selecione um pino do Arduino.",
                "Editar vídeo",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        int pino =
            (int)PinoComboBox.SelectedItem;

        VideoEditado = new VideoAtalho
        {
            Nome = nome,
            Pino = pino,
            Video = _videoOriginal.Video
        };

        DialogResult = true;

        Close();
    }

    private void BotaoCancelar_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;

        Close();
    }
}