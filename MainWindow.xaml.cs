using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using CorpoHumanoInterativo.Models;
using CorpoHumanoInterativo.Services;

namespace CorpoHumanoInterativo;

public partial class MainWindow : Window
{
    private ArduinoService? _arduinoService;

    private readonly VideoAtalhoService _videoAtalhoService = new();

    private List<VideoAtalho> _videos = new();

    private bool _videoPausado = false;

    public MainWindow()
    {
        InitializeComponent();

        CarregarPortas();

        CarregarVideos();
    }

    // ==========================================
    // COPIAR VÍDEO
    // ==========================================

    private string CopiarVideo(
        string caminhoOrigem,
        string nomeAtalho)
    {
        string pastaVideos =
            Path.Combine(
                AppContext.BaseDirectory,
                "Videos"
            );

        Directory.CreateDirectory(
            pastaVideos
        );

        string extensao =
            Path.GetExtension(
                caminhoOrigem
            );

        string nomeArquivo =
            NormalizarNome(nomeAtalho)
                .ToLowerInvariant()
            + extensao.ToLowerInvariant();

        string caminhoDestino =
            Path.Combine(
                pastaVideos,
                nomeArquivo
            );

        string origemCompleta =
            Path.GetFullPath(
                caminhoOrigem
            );

        string destinoCompleto =
            Path.GetFullPath(
                caminhoDestino
            );

        if (!origemCompleta.Equals(
            destinoCompleto,
            StringComparison.OrdinalIgnoreCase))
        {
            File.Copy(
                caminhoOrigem,
                caminhoDestino,
                true
            );
        }

        return nomeArquivo;
    }

    // ==========================================
    // ADICIONAR VÍDEO
    // ==========================================

    private void BotaoAdicionarVideo_Click(
        object sender,
        RoutedEventArgs e)
    {
        AdicionarVideoWindow janela =
            new AdicionarVideoWindow(_videos)
            {
                Owner = this
            };

        bool? resultado =
            janela.ShowDialog();

        if (resultado != true)
            return;

        if (janela.VideoCriado == null)
            return;

        if (string.IsNullOrWhiteSpace(
            janela.CaminhoVideoSelecionado))
        {
            return;
        }

        try
        {
            VideoAtalho novoVideo =
                janela.VideoCriado;

            string nomeVideo =
                CopiarVideo(
                    janela.CaminhoVideoSelecionado,
                    novoVideo.Nome
                );

            novoVideo.Video =
                nomeVideo;

            _videos.Add(
                novoVideo
            );

            _videoAtalhoService.Salvar(
                _videos
            );

            CriarBotoesVideos();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Não foi possível adicionar o vídeo.\n\n{ex.Message}",
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    // ==========================================
    // GERENCIAR VÍDEOS
    // ==========================================

    private void BotaoGerenciarVideos_Click(
        object sender,
        RoutedEventArgs e)
    {
        GerenciarVideosWindow janela =
            new GerenciarVideosWindow(_videos)
            {
                Owner = this
            };

        bool? resultado =
            janela.ShowDialog();

        if (resultado != true)
            return;

        // EDITAR

        if (janela.VideoSelecionadoParaEditar != null)
        {
            EditarVideo(
                janela.VideoSelecionadoParaEditar
            );

            return;
        }

        // EXCLUIR

        if (janela.VideoSelecionadoParaExcluir != null)
        {
            ExcluirVideo(
                janela.VideoSelecionadoParaExcluir
            );
        }
    }

    // ==========================================
    // EDITAR VÍDEO
    // ==========================================

    private void EditarVideo(
        VideoAtalho videoOriginal)
    {
        EditarVideoWindow janela =
            new EditarVideoWindow(
                videoOriginal,
                _videos
            )
            {
                Owner = this
            };

        bool? resultado =
            janela.ShowDialog();

        if (resultado != true)
            return;

        if (janela.VideoEditado == null)
            return;

        try
        {
            VideoAtalho editado =
                janela.VideoEditado;

            string videoAntigo =
                videoOriginal.Video;

            // Se foi escolhido um novo arquivo
            if (!string.IsNullOrWhiteSpace(
                janela.CaminhoNovoVideo))
            {
                string novoArquivo =
                    CopiarVideo(
                        janela.CaminhoNovoVideo,
                        editado.Nome
                    );

                editado.Video =
                    novoArquivo;
            }

            videoOriginal.Nome =
                editado.Nome;

            videoOriginal.Pino =
                editado.Pino;

            videoOriginal.Video =
                editado.Video;

            _videoAtalhoService.Salvar(
                _videos
            );

            CriarBotoesVideos();

            // Se o arquivo mudou,
            // pergunta sobre o vídeo antigo.
            if (!string.Equals(
                videoAntigo,
                videoOriginal.Video,
                StringComparison.OrdinalIgnoreCase))
            {
                PerguntarExcluirVideoAntigo(
                    videoAntigo
                );
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Não foi possível editar o vídeo.\n\n{ex.Message}",
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    // ==========================================
    // EXCLUIR VÍDEO CADASTRADO
    // ==========================================

    private void ExcluirVideo(
        VideoAtalho video)
    {
        try
        {
            bool removido =
                _videos.Remove(video);

            if (!removido)
                return;

            _videoAtalhoService.Salvar(
                _videos
            );

            CriarBotoesVideos();

            PerguntarExcluirArquivoVideo(
                video
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Não foi possível excluir o vídeo.\n\n{ex.Message}",
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    // ==========================================
    // PERGUNTAR SOBRE ARQUIVO EXCLUÍDO
    // ==========================================

    private void PerguntarExcluirArquivoVideo(
        VideoAtalho video)
    {
        bool arquivoAindaUtilizado =
            _videos.Any(
                outro =>
                    string.Equals(
                        outro.Video,
                        video.Video,
                        StringComparison.OrdinalIgnoreCase
                    )
            );

        if (arquivoAindaUtilizado)
            return;

        MessageBoxResult resposta =
            MessageBox.Show(
                $"O cadastro \"{video.Nome}\" foi excluído.\n\n" +
                $"Deseja excluir também o arquivo \"{video.Video}\"?",
                "Excluir arquivo de vídeo",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

        if (resposta != MessageBoxResult.Yes)
            return;

        ExcluirArquivoVideo(
            video.Video
        );
    }

    // ==========================================
    // VÍDEO ANTIGO APÓS EDIÇÃO
    // ==========================================

    private void PerguntarExcluirVideoAntigo(
        string nomeVideo)
    {
        bool aindaUtilizado =
            _videos.Any(
                video =>
                    string.Equals(
                        video.Video,
                        nomeVideo,
                        StringComparison.OrdinalIgnoreCase
                    )
            );

        if (aindaUtilizado)
            return;

        MessageBoxResult resposta =
            MessageBox.Show(
                $"O arquivo de vídeo foi substituído.\n\n" +
                $"Deseja excluir o arquivo antigo \"{nomeVideo}\"?",
                "Vídeo antigo",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

        if (resposta != MessageBoxResult.Yes)
            return;

        ExcluirArquivoVideo(
            nomeVideo
        );
    }

    // ==========================================
    // EXCLUIR ARQUIVO FÍSICO
    // ==========================================

    private void ExcluirArquivoVideo(
        string nomeVideo)
    {
        try
        {
            string caminhoVideo =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Videos",
                    nomeVideo
                );

            // Se o vídeo estiver sendo reproduzido,
            // é necessário pará-lo antes de apagar.
            if (VideoPlayer.Source != null)
            {
                string? videoAtual =
                    Path.GetFileName(
                        VideoPlayer.Source.LocalPath
                    );

                if (string.Equals(
                    videoAtual,
                    nomeVideo,
                    StringComparison.OrdinalIgnoreCase))
                {
                    VideoPlayer.Stop();

                    VideoPlayer.Source = null;

                    BotaoPlayPause.IsEnabled =
                        false;

                    BotaoPlayPause.Content =
                        "Pausar";

                    _videoPausado =
                        false;
                }
            }

            if (File.Exists(caminhoVideo))
            {
                File.Delete(
                    caminhoVideo
                );
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"O cadastro foi alterado, mas não foi possível excluir o arquivo de vídeo.\n\n{ex.Message}",
                "Aviso",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
    }

    // ==========================================
    // PORTAS SERIAIS
    // ==========================================

    private void CarregarPortas()
    {
        PortasComboBox.Items.Clear();

        string[] portas =
            SerialPort.GetPortNames();

        Array.Sort(
            portas
        );

        foreach (string porta in portas)
        {
            PortasComboBox.Items.Add(
                porta
            );
        }

        if (PortasComboBox.Items.Count > 0)
        {
            PortasComboBox.SelectedIndex =
                0;
        }
    }

    private void BotaoAtualizarPortas_Click(
        object sender,
        RoutedEventArgs e)
    {
        CarregarPortas();
    }

    // ==========================================
    // CONEXÃO COM ARDUINO
    // ==========================================

    private void BotaoConectarArduino_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (PortasComboBox.SelectedItem == null)
        {
            MessageBox.Show(
                "Selecione uma porta para o Arduino.",
                "Arduino",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        string porta =
            PortasComboBox
                .SelectedItem
                .ToString()!;

        ConectarArduino(
            porta
        );
    }

    private void ConectarArduino(
        string porta)
    {
        try
        {
            _arduinoService?.Dispose();

            _arduinoService =
                new ArduinoService(
                    porta
                );

            _arduinoService.ComandoRecebido +=
                Arduino_ComandoRecebido;

            _arduinoService.Iniciar();

            StatusArduino.Text =
                $"Conectado em {porta}";
        }
        catch (Exception ex)
        {
            StatusArduino.Text =
                "Desconectado";

            MessageBox.Show(
                $"Não foi possível conectar em {porta}.\n\n{ex.Message}",
                "Arduino",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
    }

    // ==========================================
    // COMANDO DO ARDUINO
    // ==========================================

    private void Arduino_ComandoRecebido(
        string comando)
    {
        Dispatcher.Invoke(() =>
        {
            VideoAtalho? video =
                EncontrarVideoPorComando(
                    comando
                );

            if (video == null)
                return;

            ReproduzirVideo(
                video.Video
            );
        });
    }

    // ==========================================
    // IDENTIFICAR BOTÃO FÍSICO
    // ==========================================

    private VideoAtalho? EncontrarVideoPorComando(
        string comando)
    {
        comando =
            comando
                .Trim()
                .ToUpperInvariant();

        // Formato principal:
        //
        // PIN_2
        // PIN_3
        // PIN_7

        if (comando.StartsWith("PIN_"))
        {
            string numeroPino =
                comando.Replace(
                    "PIN_",
                    ""
                );

            if (int.TryParse(
                numeroPino,
                out int pino))
            {
                return _videos.FirstOrDefault(
                    video =>
                        video.Pino == pino
                );
            }
        }

        // Também permite receber o nome
        // diretamente pela Serial.

        return _videos.FirstOrDefault(
            video =>
                NormalizarNome(
                    video.Nome
                )
                == comando
        );
    }

    // ==========================================
    // NORMALIZAR NOMES
    // ==========================================

    private string NormalizarNome(
        string texto)
    {
        return texto
            .Trim()
            .ToUpperInvariant()
            .Replace("Á", "A")
            .Replace("À", "A")
            .Replace("Ã", "A")
            .Replace("Â", "A")
            .Replace("Ä", "A")
            .Replace("É", "E")
            .Replace("Ê", "E")
            .Replace("Í", "I")
            .Replace("Ó", "O")
            .Replace("Ô", "O")
            .Replace("Õ", "O")
            .Replace("Ö", "O")
            .Replace("Ú", "U")
            .Replace("Ç", "C")
            .Replace(" ", "_");
    }

    // ==========================================
    // CARREGAR VÍDEOS
    // ==========================================

    private void CarregarVideos()
    {
        try
        {
            _videos =
                _videoAtalhoService.Carregar();

            CriarBotoesVideos();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Não foi possível carregar os vídeos cadastrados.\n\n{ex.Message}",
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    // ==========================================
    // CRIAR BOTÕES DINAMICAMENTE
    // ==========================================

    private void CriarBotoesVideos()
    {
        PainelVideos.Children.Clear();

        foreach (VideoAtalho video in _videos)
        {
            Button botao =
                new()
                {
                    Content = video.Nome,

                    Tag = video,

                    Width = 140,

                    Height = 50,

                    Margin =
                        new Thickness(5),

                    FontSize = 16
                };

            botao.Click +=
                BotaoVideo_Click;

            PainelVideos.Children.Add(
                botao
            );
        }
    }

    // ==========================================
    // CLIQUE NO BOTÃO DA TELA
    // ==========================================

    private void BotaoVideo_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button botao)
            return;

        if (botao.Tag is not VideoAtalho video)
            return;

        ReproduzirVideo(
            video.Video
        );
    }

    // ==========================================
    // PLAY / PAUSE
    // ==========================================

    private void BotaoPlayPause_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (VideoPlayer.Source == null)
            return;

        if (_videoPausado)
        {
            VideoPlayer.Play();

            _videoPausado =
                false;

            BotaoPlayPause.Content =
                "Pausar";
        }
        else
        {
            VideoPlayer.Pause();

            _videoPausado =
                true;

            BotaoPlayPause.Content =
                "Continuar";
        }
    }

    // ==========================================
    // REPRODUZIR VÍDEO
    // ==========================================

    private void ReproduzirVideo(
        string nomeArquivo)
    {
        string caminhoVideo =
            Path.Combine(
                AppContext.BaseDirectory,
                "Videos",
                nomeArquivo
            );

        if (!File.Exists(
            caminhoVideo))
        {
            MessageBox.Show(
                $"Vídeo não encontrado:\n{caminhoVideo}",
                "Vídeo não encontrado",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        VideoPlayer.Stop();

        VideoPlayer.Source =
            new Uri(
                caminhoVideo
            );

        VideoPlayer.Play();

        _videoPausado =
            false;

        BotaoPlayPause.Content =
            "Pausar";

        BotaoPlayPause.IsEnabled =
            true;
    }

    // ==========================================
    // FECHAMENTO
    // ==========================================

    protected override void OnClosed(
        EventArgs e)
    {
        _arduinoService?.Dispose();

        base.OnClosed(e);
    }
}