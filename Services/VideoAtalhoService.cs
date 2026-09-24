using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using CorpoHumanoInterativo.Models;

namespace CorpoHumanoInterativo.Services;

public class VideoAtalhoService
{
    private readonly string _caminhoArquivo;

    public VideoAtalhoService()
    {
        _caminhoArquivo = Path.Combine(
            AppContext.BaseDirectory,
            "videos.json"
        );
    }

    public List<VideoAtalho> Carregar()
    {
        if (!File.Exists(_caminhoArquivo))
        {
            return new List<VideoAtalho>();
        }

        string json =
            File.ReadAllText(_caminhoArquivo);

        List<VideoAtalho>? videos =
            JsonSerializer.Deserialize<List<VideoAtalho>>(
                json
            );

        return videos ?? new List<VideoAtalho>();
    }

    public void Salvar(
        List<VideoAtalho> videos)
    {
        JsonSerializerOptions opcoes = new()
        {
            WriteIndented = true
        };

        string json =
            JsonSerializer.Serialize(
                videos,
                opcoes
            );

        File.WriteAllText(
            _caminhoArquivo,
            json
        );
    }
}