# Correção de Glifo Unicode do Indicador de Submenus e Diagnóstico de Avisos

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 2026-10-28 22:45 (Horário de Brasília)
- **Status:** Concluído com Sucesso
- **Ambiente:** Windows 11, PowerShell, Unity 6000.6.0f1 (URP 2D)
- **Branch:** `unity6-startup-warnings-errors`

---

## 1. Solicitação do Usuário
Analisar os avisos emitidos no console da Unity após o teste in-game e verificar se possuem relação com as alterações dos menus:
1. `The character with Unicode value \u25B6 was not found in the [VT323-Regular Bitmap] font asset or any potential fallbacks. It was replaced by Unicode character \u25A1 in text object [Texto].`
2. `Color primaries 0 is unknown or unsupported by WindowsMediaFoundation. Falling back to default may result in color shift. VirtualArtifacts/Primary/924ed544a4e6d2644bd07d9a1f22e03a.resource`

---

## 2. Diagnóstico Técnico

### Aviso 1: Glifo `\u25B6` (`▶`)
- **Origem:** Linha 527 de `MenuPausa.cs` no método `Indicador`:
  ```csharp
  TMP_Text seta = Rotulo(pai, "▶", 16f, new Vector2(x, 0f), new Vector2(16f, 22f), TextAlignmentOptions.Center);
  ```
- **Causa:** O asset de fonte pixel art do jogo (`VT323-Regular Bitmap`) possui apenas a tabela básica ASCII e acentuações do português. O caractere Unicode `\u25B6` (triângulo preto apontando para a direita) não está contido no atlas de glifos da fonte. Quando o Canvas tentava renderizar o indicador dos subpainéis (Áudio, Vídeo, Legendas), o TextMeshPro substituía o glifo pelo quadrado de caractere não encontrado `\u25A1` (`□`) e disparava o aviso.
- **Solução:** Restaurado o caractere original `">"` (ASCII 0x3E, presente na fonte `VT323`) com a cor `corDestaque` (âmbar vintage `#F2B85C`), exatamente como no protótipo conceitual.

### Aviso 2: `Color primaries 0` (Windows Media Foundation)
- **Origem:** Reprodução de vídeo em segundo plano (`TelaPrincipalImprovisada.mp4` / `telaPrincipalV2.mp4` / `Prologo_teste.mp4`).
- **Causa:** O codec WMF da Microsoft espera tags de primárias de cor NCLX (BT.709 / BT.601) no container do MP4. Como o arquivo foi codificado com primárias 0 ("Unspecified"), o Windows emite o aviso informando que assumiu o padrão sRGB/BT.709.
- **Impacto:** Totalmente inofensivo; não interfere nos menus, na lógica do jogo ou no desempenho. O vídeo continua tocando normalmente.

---

## 3. Ações Executadas
1. **Modificação em `MenuPausa.cs`:**
   - Ajustado `Indicador(RectTransform pai, float x)` para utilizar o caractere retro `">"` em fonte `20f` com `corDestaque`.
2. **Atualização e Validação do Grafo:**
   - `.\scripts\graphify-refresh.ps1` executado com código 0.
   - `.\scripts\graphify-validate.ps1` executado com status PASS (93 scripts C# íntegros).

---

## 4. Arquivos Modificados
- `Projeto Jogo 2D/Assets/Scripts/MenuPausa.cs`

---

## 5. Validação
- **Unity Console:** Aviso de `\u25B6` eliminado; o caractere retro `>` é nativamente suportado pela fonte `VT323-Regular Bitmap`.
- **Integridade:** Grafo AST e Obsidian sincronizados.
