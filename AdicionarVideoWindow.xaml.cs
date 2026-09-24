using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

using CorpoHumanoInterativo.Models;

namespace CorpoHumanoInterativo;

public partial class AdicionarVideoWindow : Window
{
    private readonly HashSet<int> _pinosOcupados;

    private readonly HashSet<string> _nomesExistentes;

    private string? _caminhoVideoSelecionado;

    public VideoAtalho? VideoCriado { get; private set; }

    public string? CaminhoVideoSelecionado =>
        _caminhoVideoSelecionado;

    public AdicionarVideoWindow(
        IEnumerable<VideoAtalho> videosExistentes)
    {
        InitializeComponent();

        _pinosOcupados =
            videosExistentes
                .Select(video => video.Pino)
                .ToHashSet();

        _nomesExistentes =
            videosExistentes
                .Select(video => video.Nome)
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase
                );

        CarregarPinosDisponiveis();
    }

    private void CarregarPinosDisponiveis()
    {
        PinoComboBox.Items.Clear();

        for (int pino = 2; pino <= 13; pino++)
        {
            if (!_pinosOcupados.Contains(pino))
            {
                PinoComboBox.Items.Add(pino);
            }
        }

        if (PinoComboBox.Items.Count > 0)
        {
            PinoComboBox.SelectedIndex = 0;
        }
    }

    private void BotaoSelecionarVideo_Click(
        object sender,
        RoutedEventArgs e)
    {
        OpenFileDialog janela = new()
        {
            Title = "Selecione o vídeo",

            Filter =
                "Vídeos MP4 (*.mp4)|*.mp4",

            Multiselect = false
        };

        bool? resultado = janela.ShowDialog();

        if (resultado != true)
            return;

        _caminhoVideoSelecionado =
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
                "Adicionar vídeo",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        if (_nomesExistentes.Contains(nome))
        {
            MessageBox.Show(
                "Já existe um botão com esse nome.",
                "Adicionar vídeo",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        if (PinoComboBox.SelectedItem == null)
        {
            MessageBox.Show(
                "Selecione um pino do Arduino.",
                "Adicionar vídeo",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(
            _caminhoVideoSelecionado))
        {
            MessageBox.Show(
                "Selecione um vídeo.",
                "Adicionar vídeo",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        int pino =
            (int)PinoComboBox.SelectedItem;

        VideoCriado = new VideoAtalho
        {
            Nome = nome,
            Pino = pino,

            Video = Path.GetFileName(
                _caminhoVideoSelecionado
            )
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