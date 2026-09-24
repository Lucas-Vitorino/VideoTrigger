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

        // Detecta somente quando o botão é pressionado
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