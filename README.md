### Основной алгоритм:
    Начинаем обход карты из стартовой позиции вправо.
    Если в стартовой позиции содержатся единицы, то обрабатываем букву в этой области и сдвигаемся к ее правому краю.
    Иначе:
      Двигаемся по спирали, при этом учитывая направление движения. Встретив единицу, обрабатываем это слово
      (и возвращаемся в исходную позицию).
     Для каждого слова отмечаем в ?очереди? событий его "нахождение" и проверяем, находили ли его раньше. Если находили
     и в очереди событий меньше одного полного цикла поворотов, то игнорируем данное слово и продолжаем движение; иначе 
     останавливаем обход и считаем игру завершенной.
   
  _____________________
  
### Алгоритм обхода и получения слова.
    На вход необходимо направление, в  котором происходит основное движение. Каждое движение складываем в стек.
    Продолжаем движение, пока не получим: для горизонтальных 2 пустых столбца, а для вертикальных - 1 пустую строку.
    ***нужно получить верхний(нижний) левый(правый) угол слова***. Запускаем из него сканнер. Все это время нужно считать свою позицию относительно старта обхода.
    ___ несколько слов пока не рассматриваем ___
    В конце возвращаемся в исходную позицию:
      для горизонтальных и вертикальных движений считаем количество прямых и обратных, удаляем все те, что сокращаются.
      Достаем движение со стека и совершаем обратное к нему.
