# Santa Rita — produção

Para mudanças no jogo, consulte `Docs/Direcao_Arte_Narrativa_Audio.md` e os critérios vivos em `Docs/Producao_Artistica_Execucao.md`.

Skills específicas em `.agents/skills`: `santa-rita-pixel-art`, `santa-rita-narrativa`, `santa-rita-audio` e `santa-rita-qa`. Leia a skill correspondente ao trabalho; mantenha evidências no repositório. Marcus é a referência visual. Não declarar aprovação artística pelo autor sem avaliação dele.

Mapa vigente e Bíblia são as fontes de geografia e fatos; Escola foi adiada expressamente. Preserve saves pessoais e alterações preexistentes. Ferramentas e código de autoria ficam fora do player. Antes de usar Unity MCP, confirme a instância do projeto; não regenerar ou salvar cenas com trabalho não salvo.


<!-- BEGIN memoria-projeto -->
## Graphify, Obsidian e contexto compartilhado

- O progresso atual do autor é soberano. Instruções atuais, mapa/Bíblia e direção aprovada orientam requisitos; código, cenas, dados e evidências atuais comprovam implementação. Índices gerados e resumos antigos nunca anulam decisões recentes.
- Em tarefas do jogo, leia `docs/ai-memory/INDEX.md` e `CURRENT_STATE.md`, consulte as notas relevantes em `docs/architecture-vault/manual/` e use `graphify query`, `path` ou `explain` para localizar símbolos quando o índice estiver disponível. Confirme o fluxo no código real. Se a ferramenta estiver indisponível ou defasada, registre o limite e prossiga com busca direcionada.
- Após mudanças estruturais de código, atualize o grafo e o cofre com `scripts/graphify-refresh.ps1` (Windows) ou `.sh` (Bash), e valide com `scripts/graphify-validate.ps1`/`.sh`. Atualize memória e notas manuais afetadas. Mudanças só de documentação exigem atualizar as notas correspondentes; extração AST não interpreta prosa.
- `generated/` é regenerável; `manual/` contém documentação curada e não é sobrescrita pelo exportador. Os índices ficam fora de Assets/ e não entram no Player.
- Não indexar segredos, saves pessoais, caches Unity, builds ou mídias. Não executar extração semântica remota nem instalar hooks que substituam regras locais implicitamente. Refresh estrutural é local, sem LLM; relações inferidas precisam de confirmação.
- Usar caminhos relativos portáveis nos documentos. Em Windows, `Docs/` e `docs/` referem-se ao mesmo diretório: preservar ambos os conjuntos e não tentar separá-los por renomeação apenas de caixa.
- C1/C2 são organização de produção de um mundo conectado. C2 já existe até Bocha e consequência imediata; Centro e Cemitério não são trabalho futuro. Escola permanece adiada. Preservar retornos a C1 pelos acessos do mercadinho e Bar do Zé, slots explícitos de carga, lanterna manual F e save manual no orelhão.
- Respeitar o cânone R1 e não antecipar revelações de Lucas em C1. Não transformar testes históricos em validação atual nem QA técnico em aprovação artística, audição ou campanha integral.
<!-- END memoria-projeto -->
