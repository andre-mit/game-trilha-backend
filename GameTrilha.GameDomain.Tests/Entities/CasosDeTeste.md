# casos de teste
      B------B------W
      | B----x----W |
      | | x--B--x | |
      x-x-x     x-w-x
      | | x--x--x | |
      | x----x----x |
      B------x------X

      X = Espa�o vazio
      B = pe�a preta
      W = Pe�a branca

[x] Mover pe�a branca para uma posi��o valida sem moinho e sem ganhador
[x] Mover pe�a branca para posi��o invalida (Deve retornar erro)
[x] Mover pe�a branca para uma mesma posi��o (Deve retornar erro)
[x] Mover pe�a branca - Cria moinho sem ganhador
[x] Mover pe�a branca - cria moinho - ent�o remove pe�a preta - sem ganhador
[x] Mover pe�a preta - Cria moinho - ganha partida (preto)

[ ] Mover pe�a preta - Cria moinho - N�o permitir jogada branca enquanto n�o remover pe�a
[ ] Mover pe�a preta - Cria moinho - Ao atingir o tempo - passar a vez para jogador branca
[ ] Ao atingir 3 pe�as, pode movimentar para qualquer lugar vazio no tabuleiro
[ ] Ao atingir 3 pe�as para ambos jogadores, inicia contagem de movimentos e finaliza a partida com no m�ximo 10 jogadas para cada


[ ] Ao acontecer trancamento de pe�as pretas (limitar movimenta��o das pe�as brancas), o branco ganha a partida
[ ] Verificar moinho na etapa de coloca��o de pe�as no tabuleiro
    [ ] Ao colocar pe�a branca - cria moinho - remove pe�a preta
    [ ] Ao colocar pe�a branca - cria moinho - remove pe�a preta - ganha partida

[ ] No moinho - permitir remover apenas pe�as advers�rias soltas (fora de moinho)
[ ] No moinho - permitir remover pe�as advers�rias de moinho no caso de n�o ter outras op��es

[ ] Colocar pe�a em posi��o aleatoria caso estoure o tempo de colocar a pe�a (inicio da partida)