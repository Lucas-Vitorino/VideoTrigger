using System;
using System.IO.Ports;

namespace CorpoHumanoInterativo.Services;

public class ArduinoService : IDisposable
{
    private readonly SerialPort _serialPort;

    public event Action<string>? ComandoRecebido;

    public ArduinoService(
        string porta,
        int baudRate = 9600)
    {
        _serialPort =
            new SerialPort(porta, baudRate)
            {
                NewLine = "\r\n",
                ReadTimeout = 1000
            };

        _serialPort.DataReceived +=
            SerialPort_DataReceived;
    }

    public void Iniciar()
    {
        if (!_serialPort.IsOpen)
        {
            _serialPort.Open();
        }
    }

    private void SerialPort_DataReceived(
        object sender,
        SerialDataReceivedEventArgs e)
    {
        try
        {
            string comando =
                _serialPort
                    .ReadLine()
                    .Trim();

            if (!string.IsNullOrWhiteSpace(comando))
            {
                ComandoRecebido?.Invoke(comando);
            }
        }
        catch (TimeoutException)
        {
            // Ignora timeout de leitura.
        }
    }

    public void Dispose()
    {
        if (_serialPort.IsOpen)
        {
            _serialPort.Close();
        }

        _serialPort.Dispose();
    }
}