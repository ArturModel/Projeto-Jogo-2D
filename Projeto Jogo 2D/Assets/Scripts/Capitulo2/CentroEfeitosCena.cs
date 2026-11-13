using System;
using UnityEngine;

// Dados editáveis de iluminação para a composição fixa do Centro.
internal sealed class CentroEfeitosCena : MonoBehaviour
{
    [Serializable] internal sealed class Receptor
    {
        public SpriteRenderer sprite;
        public Vector3 posicaoOriginal;
        public Vector4 receiver, vento, tom;
        public Texture superficie;
        public float superficieAtiva, ignorarSombraJogador;
        public bool temReceiver, temVento, temTom, temSuperficie, temIgnorarSombra;
    }

    [Serializable] internal sealed class Silhueta
    {
        public SpriteRenderer sprite;
        public int oclusor;
        public float inclinacao, profundidade, densidade;
    }

    [Serializable] internal sealed class Luz
    {
        public Vector3 posicao;
        public float raio;
        public Color cor;
    }

    public Receptor[] receptores = Array.Empty<Receptor>();
    public SpriteRenderer[] receptoresNevoa = Array.Empty<SpriteRenderer>();
    public Silhueta[] silhuetas = Array.Empty<Silhueta>();
    public Vector4[] oclusoresMin = Array.Empty<Vector4>();
    public Vector4[] oclusoresMax = Array.Empty<Vector4>();
    public Luz[] luzes = Array.Empty<Luz>();
    public Vector2[] fontes = Array.Empty<Vector2>();
    public int indiceLuzIgreja = -1;
}
