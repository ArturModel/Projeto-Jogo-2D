using System;
using UnityEngine;

// O mesmo relógio de distância escolhe a pose e anuncia o apoio do pé.
public sealed class MarcusGait
{
    public const float ComprimentoCiclo = 1.6f;
    private float fase;
    private bool andando;
    public int Quadro { get; private set; }

    public void Reset() { fase = 7.6f; andando = false; Quadro = 0; }

    public void Avancar(float deslocamento, Action apoio)
    {
        if (deslocamento > 1f) { Reset(); return; } // Porta/checkpoint não é um passo.
        if (deslocamento < .0001f) { Reset(); return; }
        if (!andando) { fase = 7.6f; andando = true; }
        float nova = fase + deslocamento * 8f / ComprimentoCiclo;
        int ultimoApoio = -1;
        for (int i = Mathf.FloorToInt(fase) + 1; i <= Mathf.FloorToInt(nova); i++)
            if (i % 4 == 0) ultimoApoio = i % 8;
        fase = nova % 8f;
        // Mesmo após um frame demorado, mostrar o apoio antes da passagem.
        // Contatos perdidos no intervalo não viram vários sons no mesmo frame.
        Quadro = ultimoApoio >= 0 ? ultimoApoio : Mathf.FloorToInt(fase);
        if (ultimoApoio >= 0) apoio?.Invoke();
    }
}
