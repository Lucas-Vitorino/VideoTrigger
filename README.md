# 🎬 VideoTrigger

O **VideoTrigger** é um aplicativo desktop desenvolvido em **C# com .NET e WPF** que permite reproduzir vídeos por meio de **botões físicos conectados a um Arduino**.

O projeto nasceu como uma solução para uma maquete interativa e evoluiu para uma aplicação genérica: cada botão físico pode ser associado a um vídeo diferente sem necessidade de alterar o código do Arduino ou recompilar a aplicação.

---

## 📌 Visão geral

O funcionamento é simples:

```text
Botão físico
    ↓
Arduino
    ↓
Comando serial: PIN_X
    ↓
VideoTrigger
    ↓
videos.json
    ↓
Vídeo associado ao pino
    ↓
Reprodução
```

Exemplo:

```text
Botão no pino D7
      ↓
Arduino envia PIN_7
      ↓
VideoTrigger procura o cadastro do pino 7
      ↓
Reproduz o vídeo correspondente
```

---

## ✨ Funcionalidades

- Conexão com Arduino através de porta serial
- Detecção das portas COM disponíveis
- Associação entre pinos digitais e vídeos
- Reprodução de vídeos por botões físicos
- Reprodução pelos botões da própria interface
- Pausar e continuar a reprodução
- Cadastro de novos vídeos
- Edição de nome, pino e arquivo de vídeo
- Exclusão de cadastros
- Exclusão opcional do arquivo de vídeo
- Persistência das configurações em JSON
- Interface desktop construída com WPF
- Distribuição self-contained para Windows

---

## 🛠️ Tecnologias utilizadas

- **C#**
- **.NET**
- **WPF**
- **Arduino**
- **Comunicação Serial**
- **System.IO.Ports**
- **JSON**
- **Git / GitHub**

---

## 🧱 Arquitetura

O projeto separa responsabilidades em modelos, serviços e interface.

```text
VideoTrigger
│
├── Models
│   └── VideoAtalho.cs
│
├── Services
│   ├── ArduinoService.cs
│   └── VideoAtalhoService.cs
│
├── Views
│   ├── MainWindow
│   ├── AdicionarVideoWindow
│   ├── EditarVideoWindow
│   └── GerenciarVideosWindow
│
├── Videos
│
└── videos.json
```

### `ArduinoService`

Responsável pela comunicação serial com o Arduino.

A aplicação recebe comandos no formato:

```text
PIN_2
PIN_3
PIN_4
```

### `VideoAtalhoService`

Responsável por carregar e salvar os vínculos entre:

```text
Nome + Pino + Vídeo
```

no arquivo `videos.json`.

### `VideoAtalho`

Modelo utilizado para representar cada configuração:

```csharp
public class VideoAtalho
{
    public string Nome { get; set; } = string.Empty;
    public int Pino { get; set; }
    public string Video { get; set; } = string.Empty;
}
```

---

## 💾 Exemplo de configuração

O arquivo `videos.json` armazena os vídeos cadastrados:

```json
[
  {
    "Nome": "Apresentação",
    "Pino": 2,
    "Video": "apresentacao.mp4"
  },
  {
    "Nome": "Experimento",
    "Pino": 3,
    "Video": "experimento.mp4"
  }
]
```

O usuário não precisa editar esse arquivo manualmente. Os registros podem ser adicionados, alterados e removidos pela própria interface.

---

## 🔌 Arduino

O Arduino não conhece os nomes ou arquivos dos vídeos.

Sua responsabilidade é apenas detectar o botão pressionado e informar o pino correspondente ao aplicativo.

```cpp
const int PINO_INICIAL = 2;
const int PINO_FINAL = 13;

bool estadoAnterior[14];

void setup()
{
    Serial.begin(9600);

    for (int pino = PINO_INICIAL; pino <= PINO_FINAL; pino++)
    {
        pinMode(pino, INPUT_PULLUP);
        estadoAnterior[pino] = HIGH;
    }
}

void loop()
{
    for (int pino = PINO_INICIAL; pino <= PINO_FINAL; pino++)
    {
        bool estadoAtual = digitalRead(pino);

        if (estadoAnterior[pino] == HIGH &&
            estadoAtual == LOW)
        {
            Serial.print("PIN_");
            Serial.println(pino);
        }

        estadoAnterior[pino] = estadoAtual;
    }

    delay(30);
}
```

---

## 🔘 Ligação do botão

O projeto utiliza `INPUT_PULLUP`, portanto o botão pode ser ligado diretamente entre um pino digital e o GND.

```text
D2 -------- BOTÃO -------- GND
```

Podem ser utilizados os pinos **D2 até D13**.

Os pinos **D0 e D1** não são utilizados, pois estão relacionados à comunicação serial.

---

## ▶️ Executando o projeto

### Pré-requisitos para desenvolvimento

- Windows
- .NET SDK
- Arduino ou placa compatível
- Arduino IDE para carregar o firmware na placa

Clone o repositório:

```bash
git clone https://github.com/SEU-USUARIO/VideoTrigger.git
```

Entre na pasta:

```bash
cd VideoTrigger
```

Restaure as dependências:

```bash
dotnet restore
```

Execute:

```bash
dotnet run
```

---

## 🖥️ Como usar

1. Conecte o Arduino ao computador.
2. Abra o VideoTrigger.
3. Selecione a porta COM correspondente.
4. Clique em **Conectar**.
5. Cadastre um vídeo e associe-o a um pino.
6. Conecte um botão físico ao mesmo pino.
7. Pressione o botão.
8. O vídeo será reproduzido automaticamente.

---

## ➕ Cadastro de vídeos

Pela opção **Adicionar vídeo**, o usuário informa:

- nome do botão;
- pino do Arduino;
- arquivo MP4.

O VideoTrigger copia o arquivo para a pasta `Videos` e atualiza automaticamente o `videos.json`.

---

## ✏️ Edição e exclusão

Em **Gerenciar vídeos**, é possível:

- alterar o nome;
- alterar o pino;
- substituir o vídeo;
- excluir o cadastro;
- excluir opcionalmente o arquivo físico.

---

## 📦 Publicação

Para gerar uma versão para Windows 64 bits sem exigir uma instalação separada do .NET:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -o ".\VideoTrigger-Publicacao"
```

> A pasta de publicação não deve ser versionada junto com o código-fonte. Para distribuir o aplicativo, prefira disponibilizar o ZIP da publicação através da área **Releases** do GitHub.

---

## 💡 Possíveis aplicações

O VideoTrigger pode ser utilizado em:

- feiras de Ciências;
- exposições;
- museus;
- maquetes interativas;
- totens multimídia;
- atividades educacionais;
- instalações artísticas;
- demonstrações de produtos;
- projetos Maker;
- protótipos com Arduino.

---

## 🎯 Conceitos aplicados

Este projeto foi desenvolvido com foco na prática de:

- Programação orientada a objetos
- Separação de responsabilidades
- Comunicação entre hardware e software
- Persistência de dados em JSON
- Manipulação de arquivos
- Programação orientada a eventos
- Comunicação serial
- Interfaces desktop com WPF
- Versionamento com Git

---

## 🚀 Próximas evoluções

Algumas melhorias planejadas para versões futuras:

- suporte a outros formatos de mídia;
- tela cheia automática;
- configuração de volume por vídeo;
- opção de repetir vídeos;
- detecção automática do Arduino;
- melhoria da experiência visual da interface;
- instalador para Windows;
- armazenamento das configurações em diretório próprio do usuário.

---

## 📄 Licença

Este projeto foi desenvolvido para fins educacionais e de portfólio.

---

## 👨‍💻 Autor

Desenvolvido por **Lucas Vitorino**.

Projeto criado como estudo prático de integração entre **C#/.NET, WPF e Arduino**.
