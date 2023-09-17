# casos de teste
      B------B------W
      | B----x----W |
      | | x--B--x | |
      x-x-x     x-w-x
      | | x--x--x | |
      | x----x----x |
      B------x------X

      X = Espaço vazio
      B = peça preta
      W = Peça branca

[x] Mover peça branca para uma posição valida sem moinho e sem ganhador
[x] Mover peça branca para posição invalida (Deve retornar erro)
[x] Mover peça branca para uma mesma posição (Deve retornar erro)
[x] Mover peça branca - Cria moinho sem ganhador
[x] Mover peça branca - cria moinho - então remove peça preta - sem ganhador
[x] Mover peça preta - Cria moinho - ganha partida (preto)

[ ] Mover peça preta - Cria moinho - Não permitir jogada branca enquanto não remover peça
[ ] Mover peça preta - Cria moinho - Ao atingir o tempo - passar a vez para jogador branca
[ ] Ao atingir 3 peças, pode movimentar para qualquer lugar vazio no tabuleiro
[ ] Ao atingir 3 peças para ambos jogadores, inicia contagem de movimentos e finaliza a partida com no máximo 10 jogadas para cada


[ ] Ao acontecer trancamento de peças pretas (limitar movimentação das peças brancas), o branco ganha a partida
[ ] Verificar moinho na etapa de colocação de peças no tabuleiro
    [ ] Ao colocar peça branca - cria moinho - remove peça preta
    [ ] Ao colocar peça branca - cria moinho - remove peça preta - ganha partida

[ ] No moinho - permitir remover apenas peças adversárias soltas (fora de moinho)
[ ] No moinho - permitir remover peças adversárias de moinho no caso de não ter outras opções

[ ] Colocar peça em posição aleatoria caso estoure o tempo de colocar a peça (inicio da partida)