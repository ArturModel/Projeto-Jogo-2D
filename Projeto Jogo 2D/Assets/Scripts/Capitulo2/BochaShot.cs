using UnityEngine;

public enum BochaThrowType { Ponto, Rafa, Volo }
public struct BochaShot
{
    public float x, altura, vertical;
    public Vector2 velocidade;
    public Vector3 giro;
    public BochaThrowType tipo;
    public string intencao;
}
public static class BochaLaunch
{
    public static BochaShot Preparar(BochaThrowType tipo, float x, Vector2 direcao, float forca)
    {
        float p = Mathf.Clamp01(forca);
        float angle = Mathf.Clamp(Mathf.Atan2(direcao.x, Mathf.Max(.01f, direcao.y)), -.58f, .58f);
        Vector2 heading = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        float speed = tipo == BochaThrowType.Ponto ? Mathf.Lerp(1.6f, 7.2f, p) : tipo == BochaThrowType.Rafa ? Mathf.Lerp(3.8f, 12.5f, p) : Mathf.Lerp(3.2f, 10.2f, p);
        Vector2 v = heading * speed;
        float rolling = tipo == BochaThrowType.Ponto ? 1 : tipo == BochaThrowType.Rafa ? .2f : .1f;
        return new BochaShot { tipo = tipo, x = Mathf.Clamp(x, -2.6f, 2.6f), velocidade = v,
            altura = tipo == BochaThrowType.Volo ? .65f : 0, vertical = tipo == BochaThrowType.Volo ? Mathf.Lerp(4.2f, 7f, p) : 0,
            giro = new Vector3(v.y / BochaSimulation.RaioBocha, 0, -v.x / BochaSimulation.RaioBocha) * rolling };
    }
    public static float ForcaPonto(float distancia) => Mathf.Clamp01((Mathf.Sqrt(2 * BochaSimulation.ResistenciaRolagem * distancia) - 1.6f) / 5.6f);
}
