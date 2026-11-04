using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable] public sealed class MarcusNotebookEntry { public string id,title,body,requires; }
[Serializable] public sealed class MarcusNotebookCatalog { public MarcusNotebookEntry[] entries; }

// Projeção editorial de registros já adquiridos. Não altera provas/diálogos/objetivos.
public static class MarcusNotebookContent
{
    private static MarcusNotebookCatalog catalog;
    public static List<MarcusNotebookEntry> Read()
    {
        var result=new List<MarcusNotebookEntry>{new MarcusNotebookEntry{id="abertura",title="19 de outubro, 2004",body="Voltei pra Santa Rita porque o vô não atendia. Na estrada, comecei a ensaiar o que ia dizer quando ele abrisse a porta. Uma bronca pequena, daquelas que a gente dá só pra esconder o susto.\n\nTrouxe este caderno. Se eu deixar tudo só na cabeça, sei como é: depois cada lembrança arruma um jeito mais fácil de ser contada."}};
        foreach(var entry in GameState.ObterEntradasDiario())
        {
            var prose=Project(entry);if(prose!=null)result.Add(prose);
        }
        return result;
    }
    public static MarcusNotebookEntry Project(EntradaDiario entry)
    {
        if(entry==null||string.Equals(entry.categoria,"item",StringComparison.OrdinalIgnoreCase))return null;
        if(catalog==null)
        {
            var json=Resources.Load<TextAsset>("Notebook/Entries");
            catalog=json!=null?JsonUtility.FromJson<MarcusNotebookCatalog>(json.text):new MarcusNotebookCatalog{entries=Array.Empty<MarcusNotebookEntry>()};
        }
        if(entry.id=="linha_do_tempo_adilio")return new MarcusNotebookEntry{id=entry.id,title="Pra não misturar os horários",body="Escrevi os horários que consegui juntar. Alguns vieram de papéis, outros da lembrança dos vizinhos. Não vou fingir que todos têm a mesma precisão.\n\n"+Plain(entry.descricao)+"\n\nEntre uma linha e outra tem coisa que eu ainda não sei."};
        MarcusNotebookEntry found=null;
        foreach(var text in catalog.entries)
            if(text.id==entry.id&&(string.IsNullOrEmpty(text.requires)||GameState.Tem(text.requires)))found=text;
        if(found!=null)return found;
        // Compatibilidade com notas legadas ou futuras ainda sem versão editorial própria.
        // Preserva a fonte como transcrição, sem inventar memória ou informação.
        return new MarcusNotebookEntry{id=entry.id,title=Plain(entry.titulo),body="Deixei isso anotado do jeito que consegui guardar:\n\n“"+Plain(entry.descricao).Split(new[]{"\n\nFonte:"},StringSplitOptions.None)[0]+"”\n\nUma anotação não é a mesma coisa que uma certeza. Ainda preciso separar o que vi do que fiquei imaginando."};
    }
    private static string Plain(string text)=>Regex.Replace(text??"","<[^>]+>","");
}
