# Trabalho_Pratico_2

📝 Descrição
Jogo de plataforma 2D desenvolvido em C# com MonoGame, onde o jogador controla um esqueleto guerreiro que deve derrotar inimigos e enfrentar um chefe final em um cenário com múltiplos andares e plataformas móveis.

🎮 Funcionalidades Principais
Sistema de Combate: Ataques corpo-a-corpo com efeitos de knockback

Inimigos Variados: Slimes comuns, morcegos voadores e um Boss

Mecânicas de Plataforma: Elevadores móveis, pisos em diferentes alturas e paredes colidíveis

Sistema de Vida: Barra de vida visual e efeitos de dano

Animação Completa: Sprites animados para todos os personagens e ações

Sons e Música: Efeitos sonoros para ações e trilha sonora ambiente

🛠️ Requisitos do Sistema
.NET 6.0

MonoGame

Placa de vídeo compatível com DirectX 11

500MB de espaço livre em disco

⚙️ Como Executar
Clone o repositório:

bash
git clone https://github.com/JoaoZach/Trabalho_Pratico_2.git
Navegue até o diretório do projeto:

bash
cd Trabalho_Pratico_2
Execute o jogo:

bash
dotnet run
🕹️ Controles
Tecla	Ação
← →	Movimento horizontal
↑	Pular
Z	Atacar
X 	Dash
ESC	Sair do jogo
🏗️ Estrutura do Projeto
Trabalho_Pratico_2/
├── Content/               # Assets do jogo (sprites, sons)
├── Game1.cs               # Classe principal do jogo
├── Player.cs              # Lógica do jogador
├── Enemy.cs               # Inimigos comuns
├── BatEnemy.cs            # Inimigos voadores
├── SlimeBoss.cs           # Chefe final
├── Elevator.cs            # Plataformas móveis
├── Platform.cs            # Plataformas estáticas
├── Animation.cs           # Sistema de animação
├── AnimationManager.cs    # Gerenciador de animações
└── Room.cs                # Gerenciamento de salas
🎨 Assets Utilizados
Sprites: Personagens e inimigos com animações completas

Sons: Efeitos sonoros para ataques, dano e morte

Música: Trilha sonora ambiente para o jogo

📊 Mecânicas do Jogo
Sistema de Andares: Três níveis verticais conectados por plataformas

Elevadores: Plataformas móveis que transportam o jogador entre andares

Combate:

Ataque básico com tecla Z

Inimigos recebem knockback quando atingidos

Sistema de vida para jogador e inimigos

IA Inimiga:

Slimes: Movimento patrulhado com detecção de paredes

Morcegos: Perseguição ao jogador com linha de visão

Slime Boss: Comportamento único com mais vida e dano

📜 Regras do Jogo
Derrote todos os inimigos para avançar

O jogo termina quando o jogador morre ou derrota o chefe final

Cuidado com as bordas das plataformas

✨ Créditos
Desenvolvido por: 
David Guimarães 31460
João Faria 25590
Pedro Cunha 31462

Assets: XtremeFreddy
Sound Effect by <a href="https://pixabay.com/users/xtremefreddy-32332307/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=145285">Gaston A-P</a> from <a href="https://pixabay.com/sound-effects//?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=145285">Pixabay</a>
https://pixabay.com/sound-effects/game-music-loop-7-145285/

Karim-Nessim
Sound Effect by <a href="https://pixabay.com/users/karim-nessim-40448081/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=260274">karim nessim</a> from <a href="https://pixabay.com/sound-effects//?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=260274">Pixabay</a>
https://pixabay.com/sound-effects/sword-sound-260274/

666HeroHero
Sound Effect by <a href="https://pixabay.com/users/666herohero-25759907/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=131480">666HeroHero</a> from <a href="https://pixabay.com//?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=131480">Pixabay</a>
https://pixabay.com/sound-effects/monster-death-grunt-131480/

freesound_community
Sound Effect by <a href="https://pixabay.com/users/freesound_community-46691455/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=6462">freesound_community</a> from <a href="https://pixabay.com//?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=6462">Pixabay</a>
https://pixabay.com/sound-effects/cartoon-jump-6462/

Homemade_SFX
Sound Effect by <a href="https://pixabay.com/users/homemade_sfx-47000485/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=262618">Homemade_SFX</a> from <a href="https://pixabay.com//?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=262618">Pixabay</a>
https://pixabay.com/sound-effects/slap-hurt-pain-sound-effect-262618/

freesound_community
Sound Effect by <a href="https://pixabay.com/users/freesound_community-46691455/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=47810">freesound_community</a> from <a href="https://pixabay.com//?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=47810">Pixabay</a>
https://pixabay.com/sound-effects/player-hurt-47810/

https://pt.pinterest.com/pin/67061481945183537/

https://pt.pinterest.com/pin/492159065549124620/

📄 Licença
Este projeto é para fins educacionais e acadêmicos. Não destinado para distribuição comercial.
