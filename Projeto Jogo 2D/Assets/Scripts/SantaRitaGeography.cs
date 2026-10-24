using System;
using System.Collections.Generic;
using UnityEngine;

// Bíblia R1 §11: Prefeitura no antigo rótulo Escola; Capela no meio inferior.
// Escola continua futura. Coordenadas UV têm origem no canto inferior esquerdo.
public static class SantaRitaGeography
{
    public static readonly Vector2 Prefeitura = new Vector2(88, 66);
    public static readonly Vector2 EntradaPrefeitura = Prefeitura + Vector2.down * .55f;
    public static readonly Vector2 RetornoPrefeitura = Prefeitura + Vector2.down * 2.5f;

    public static Vector2 MundoC2ParaUV(Vector2 position) =>
        new Vector2((position.x / .32f + 200) / 676f, 1f - (355 - position.y / .32f) / 503f);

    private static readonly Dictionary<string, Vector2> points = new Dictionary<string, Vector2>
    {
        { "entrada", new Vector2(.110f, .640f) }, { "posto", new Vector2(.145f, .630f) },
        { "orelhao_sul", new Vector2(.208f, .618f) }, { "mercadinho", new Vector2(.378f, .595f) },
        { "casa_avo", new Vector2(.255f, .765f) }, { "bar_ze", new Vector2(.398f, .885f) },
        { "orelhao_bar", new Vector2(.435f, .865f) }, { "centro", MundoC2ParaUV(new Vector2(44, 48)) },
        { "canto_se", MundoC2ParaUV(new Vector2(5, 5)) }, { "capela", new Vector2(.520f, .175f) },
        { "truco", new Vector2(.855f, .335f) }, { "cemiterio", MundoC2ParaUV(new Vector2(64, 99)) },
        { "prefeitura", MundoC2ParaUV(Prefeitura) }, { "escola", new Vector2(.622f, .595f) },
        { "clube", new Vector2(.868f, .715f) }, { "igreja", MundoC2ParaUV(new Vector2(44.8f, 64)) },
        { "rodoviaria", MundoC2ParaUV(new Vector2(68, 22)) },
        { "orelhao_centro", MundoC2ParaUV(new Vector2(40, 41.8f)) },
        { "orelhao_bocha", MundoC2ParaUV(new Vector2(12.7f, 4.6f)) },
        { "oficina", MundoC2ParaUV(new Vector2(11.2f, 33.6f)) },
        { "costura", MundoC2ParaUV(new Vector2(21.8f, 28.5f)) },
        { "abrigo", MundoC2ParaUV(new Vector2(78.5f, 101)) }
    };

    public static Vector2 UV(string id) => points.TryGetValue(id, out var value) ? value :
        throw new ArgumentException("Marco geográfico desconhecido: " + id);

    public static bool CenaC2(string scene) => IdDaCena(scene) != null;
    public static string IdDaCena(string scene)
    {
        switch (scene)
        {
            case "CentroSantaRita": return "centro";
            case "IgrejaMatriz": return "igreja";
            case "PrefeituraSantaRita": return "prefeitura";
            case "RodoviariaSantaRita": return "rodoviaria";
            case "MercadinhoDeposito": return "mercadinho";
            case "OficinaRadio": return "oficina";
            case "CasaCostureira": return "costura";
            case "SociedadeBeiraRio": return "canto_se";
            case "AbrigoCemiterio": return "abrigo";
            case "CemiterioSantaRita": return "cemiterio";
            default: return null;
        }
    }

    public static Vector2 ObjetivoAtual()
    {
        if (GameState.Tem("c2_iniciado"))
        {
            if (GameState.Tem(Capitulo2Director.Vitoria)) return UV("orelhao_bocha");
            if (!GameState.Tem("c2_protocolo_adilio") || !GameState.Tem("c2_registro_manutencao")) return UV("prefeitura");
            if (!GameState.Tem("c2_padre_testemunho")) return UV("igreja");
            if (!GameState.Tem("c2_cemiterio_comparado")) return UV("cemiterio");
            return UV("canto_se");
        }
        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca)) return UV("centro");
        if (GameState.Tem(Flags.Pista_RumoBarDoZe) || GameState.Tem(Flags.OuviuRelatoAtendente)) return UV("bar_ze");
        if (GameState.Tem(Flags.AchouReciboMercadinho)) return UV("mercadinho");
        return UV("casa_avo");
    }
    public static Vector2 ConverterCapitulo1(Vector3 pos, out string nomeLocal)
    {
        float x = pos.x;
        float y = pos.y;

        Capitulo1Director.ZonaId zona = Capitulo1Director.ZonaAtual;
        if (Capitulo1Director.TryObterInfoZona(zona, out Capitulo1Director.InfoZona info))
        {
            float tX = Mathf.InverseLerp(info.min.x + 2f, info.max.x - 2f, x);
            float tY = Mathf.InverseLerp(info.min.y + 2f, info.max.y - 2f, y);

            switch (zona)
            {
                case Capitulo1Director.ZonaId.Zona1_Rodovia:
                {
                    float u = Mathf.Lerp(0.085f, 0.132f, tX);
                    float v = Mathf.Lerp(0.642f, 0.634f, tX);
                    nomeLocal = "Rodovia de Entrada — Santa Rita";
                    return new Vector2(u, v);
                }

                case Capitulo1Director.ZonaId.Zona2_Posto:
                {
                    float u = Mathf.Lerp(0.138f, 0.228f, tX);
                    float v = Mathf.Lerp(0.633f, 0.616f, tX);
                    nomeLocal = tX < 0.58f ? "Avenida Sul — Posto de Gasolina" : "Avenida Sul — Antiga Venda";
                    return new Vector2(u, v);
                }

                case Capitulo1Director.ZonaId.Interior_Posto:
                {
                    nomeLocal = "Interior — Conveniência do Posto";
                    return new Vector2(0.145f, 0.630f);
                }

                case Capitulo1Director.ZonaId.Zona3_Encruzilhada:
                {
                    float u = Mathf.Lerp(0.245f, 0.382f, tX);
                    float v = Mathf.Lerp(0.612f, 0.595f, tX) + Mathf.Max(0f, tY - 0.45f) * 0.13f;
                    if (tY > 0.62f)
                        nomeLocal = "Subida de Barro para o Bairro";
                    else if (tX > 0.45f)
                        nomeLocal = "Encruzilhada — Mercadinho Santa Rita";
                    else
                        nomeLocal = "Encruzilhada Principal";
                    return new Vector2(Mathf.Clamp(u, 0.05f, 0.95f), Mathf.Clamp(v, 0.08f, 0.94f));
                }

                case Capitulo1Director.ZonaId.Interior_Mercadinho:
                {
                    nomeLocal = "Interior — Mercadinho Santa Rita";
                    return new Vector2(0.378f, 0.595f);
                }

                case Capitulo1Director.ZonaId.Zona4_Bairro:
                {
                    float u = Mathf.Lerp(0.185f, 0.285f, tX);
                    float v = Mathf.Lerp(0.725f, 0.795f, tY);
                    bool pertoCasaAvo = x >= 424f && x <= 438f && y >= 15f && y <= 27f;
                    nomeLocal = pertoCasaAvo ? "Bairro das Casas — Casa de Adílio (Nº 40)" : "Bairro das Casas (Ruas de Barro)";
                    return new Vector2(u, v);
                }

                case Capitulo1Director.ZonaId.Interior_CasaDoAvo:
                {
                    nomeLocal = "Interior — Casa de Adílio (Nº 40)";
                    return new Vector2(0.255f, 0.765f);
                }

                case Capitulo1Director.ZonaId.Zona5_RuaNorteBar:
                {
                    float u = Mathf.Lerp(0.345f, 0.465f, tX);
                    float v = Mathf.Lerp(0.855f, 0.885f, 1f - Mathf.Abs(tX - 0.5f) * 1.5f);
                    nomeLocal = tX > 0.78f ? "Rua Norte — Portão de Acesso ao Centro" : "Rua Norte — Exterior do Bar do Zé";
                    return new Vector2(u, v);
                }

                case Capitulo1Director.ZonaId.Interior_BarDoZe:
                {
                    nomeLocal = "Interior — Bar do Zé (Mesa de Sinuca)";
                    return new Vector2(0.398f, 0.885f);
                }
            }
        }

        nomeLocal = "Entrada de Santa Rita";
        return new Vector2(0.110f, 0.640f);
    }

}
