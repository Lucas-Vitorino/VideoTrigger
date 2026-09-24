using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CorpoHumanoInterativo.Models;

namespace CorpoHumanoInterativo;

public partial class GerenciarVideosWindow : Window
{
    public VideoAtalho? VideoSelecionadoParaEditar
    {
        get;
        private set;
    }

    public VideoAtalho? VideoSelecionadoParaExcluir
    {
        get;
        private set;
    }

    public GerenciarVideosWindow(
        IEnumerable<VideoAtalho> videos)
    {
        InitializeComponent();

        VideosDataGrid.ItemsSource =
            videos.ToList();
    }

    private void BotaoEditar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (VideosDataGrid.SelectedItem
            is not VideoAtalho video)
        {
            MessageBox.Show(
                "Selecione um vídeo para editar.",
                "Gerenciar vídeos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        VideoSelecionadoParaEditar =
            video;

        DialogResult = true;

        Close();
    }

    private void BotaoExcluir_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (VideosDataGrid.SelectedItem
            is not VideoAtalho video)
        {
            MessageBox.Show(
                "Selecione um vídeo para excluir.",
                "Gerenciar vídeos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        MessageBoxResult confirmacao =
            MessageBox.Show(
                $"Deseja realmente excluir \"{video.Nome}\"?",
                "Confirmar exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

        if (confirmacao != MessageBoxResult.Yes)
            return;

        VideoSelecionadoParaExcluir =
            video;

        DialogResult = true;

        Close();
    }

    private void BotaoFechar_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;

        Close();
    }
}