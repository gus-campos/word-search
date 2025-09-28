import {
  Box,
  Button,
  Flex,
  Group,
  LoadingOverlay,
  Modal,
  NumberInput,
  Paper,
  SimpleGrid,
  Stack,
  Text,
  TextInput,
} from '@mantine/core';
import { useForm } from '@mantine/form';
import { useDisclosure } from '@mantine/hooks';
import { useEffect, useState } from 'react';
import { z } from 'zod';

const WordSearchResponseSchema = z.object({
  grid: z.array(z.array(z.string())),
  words: z.array(z.string()),
});

type WordSearch = z.infer<typeof WordSearchResponseSchema>;

const createWordSearch = async (wordSearchParams: WordSearchParams) => {
  let wordSearch: WordSearch | null = null;

  const response = await fetch('http://localhost:5235/api/WordSearch', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      rows: wordSearchParams.rows,
      columns: wordSearchParams.columns,
      numberOfWords: wordSearchParams.words,
      topic: wordSearchParams.topic,
    }),
  });

  if (!response.ok) {
    throw new Error(
      `Erro na requisição: ${response.status} ${response.statusText}`
    );
  }

  wordSearch = WordSearchResponseSchema.parse(await response.json());

  return wordSearch;
};

type WordSearchParams = {
  rows: number;
  columns: number;
  words: number;
  topic: string;
};

type Coord = {
  row: number;
  column: number;
};

const indexToCoord = (index: number, wordSearchParams: WordSearchParams) => {
  const column = index % wordSearchParams.columns;
  const row = Math.floor(index / wordSearchParams.columns);

  const coord: Coord = { row, column };
  return coord;
};

const coordToIndex = (coord: Coord, wordSearchParams: WordSearchParams) => {
  return coord.row * wordSearchParams.columns + coord.column;
};

const initialParams: WordSearchParams = {
  rows: 40,
  columns: 40,
  words: 10,
  topic: '',
};

export function WordSearch() {
  const [wordSearchParams, setWordSearchParams] =
    useState<WordSearchParams>(initialParams);
  const [wordSearch, setWordSearch] = useState<WordSearch | null>(null);
  const [opened, { open, close }] = useDisclosure(false);
  const [lettersTried, setLettersTried] = useState<[number, number]>([-1, -1]);
  const [wordsFound, setWordsFound] = useState<(number[] | null)[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const areCoordsEqual = (a: Coord, b: Coord) => {
    return a.row === b.row && a.column == b.column;
  };

  const incrementCoord = (start: Coord, increment: Coord) => {
    const incremented: Coord = {
      row: start.row + increment.row,
      column: start.column + increment.column,
    };
    return incremented;
  };

  const tryWord = (nextMarked: [number, number]) => {
    if (nextMarked[0] === -1 || nextMarked[1] === -1) return;

    const startCoord = indexToCoord(nextMarked[0], wordSearchParams);
    const endCoord = indexToCoord(nextMarked[1], wordSearchParams);

    const delta: Coord = {
      row: endCoord.row - startCoord.row,
      column: endCoord.column - startCoord.column,
    };

    const squared = Math.abs(delta.column) === Math.abs(delta.row);
    const linear = delta.column === 0 || delta.row === 0;

    if (!linear && !squared) return;

    if (!wordSearch) throw new Error('wordSearch nulo');

    const letters: string[] = [];
    const coords: Coord[] = [];

    const length = Math.max(Math.abs(delta.row), Math.abs(delta.column));
    const step: Coord = {
      row: delta.row / length,
      column: delta.column / length,
    };

    for (
      let coord = startCoord;
      !areCoordsEqual(coord, incrementCoord(endCoord, step));
      coord = incrementCoord(coord, step)
    ) {
      // console.log(coord, startCoord, endCoord);
      coords.push(coord);
      letters.push(wordSearch.grid[coord.row][coord.column]);
    }

    for (let i = 0; i < wordSearch.words.length; i++) {
      const currentWord = wordSearch.words[i];

      const matches =
        currentWord === letters.join('') ||
        currentWord === letters.reverse().join('');

      if (matches) {
        setWordsFound((prev) =>
          prev.map((word, index) => {
            return index == i
              ? coords.map((coord) => coordToIndex(coord, wordSearchParams))
              : word;
          })
        );

        setLettersTried([-1, -1]);

        return;
      }
    }
  };

  const tryLetter = (letterIndex: number) => {
    let nextMarked: [number, number];
    if (lettersTried[0] != -1 && lettersTried[1] != -1) {
      nextMarked = [-1, -1];
    } else if (lettersTried.includes(letterIndex)) {
      const diff = lettersTried.find((index) => index !== letterIndex);
      nextMarked = [-1, diff!];
    } else {
      nextMarked = [lettersTried[1], letterIndex];
    }

    setLettersTried(nextMarked);

    if (nextMarked[0] !== -1 && nextMarked[1] !== -1) {
      tryWord(nextMarked);
      setLettersTried([-1, -1]);
    }
  };

  const letterFound = (letterIndex: number) => {
    return wordsFound.some((word) =>
      word?.some((wordLetterIndex) => wordLetterIndex == letterIndex)
    );
  };

  const wordFound = (wordIndex: number) => {
    return wordSearch?.words.some(
      (word, index) => index === wordIndex && wordsFound[wordIndex] != null
    );
  };

  const handleSubmit = (wordSearchParams: WordSearchParams) => {
    setError(null);
    setLoading(true);
    createWordSearch(wordSearchParams)
      .then((ws) => {
        setWordSearch(ws);
        setWordsFound(Array(ws.words.length).fill(null));
        setLoading(false);
      })
      .catch(() => {
        setError('Erro ao gerar. Falta de conexão, ou palavras demais.');
      });
  };

  useEffect(() => {
    handleSubmit(wordSearchParams);
  }, []);

  const form = useForm({
    initialValues: initialParams,
  });

  return (
    <>
      <div>
        {error ? (
          `ERRO: ${error}`
        ) : loading ? (
          <LoadingOverlay visible={true} />
        ) : (
          <Paper m="lg" p="lg">
            <Group justify="space-around" align="center">
              <SimpleGrid
                cols={Math.floor(wordSearch!.words.length / 5 + 1)}
                spacing="xl"
                verticalSpacing="xs"
              >
                {wordSearch!.words.map((word, index) => (
                  <Box key={index}>
                    <Text
                      style={{
                        textDecoration: wordFound(index)
                          ? 'line-through'
                          : 'unset',
                      }}
                    >
                      {word}
                    </Text>
                  </Box>
                ))}
              </SimpleGrid>
            </Group>

            <Group justify="center" align="center" mt="md">
              <SimpleGrid
                cols={wordSearch!.grid[0].length}
                spacing="xs"
                verticalSpacing="xs"
                style={{
                  padding: '20px',
                  border: 'solid 1px black',
                  borderRadius: '10px',
                }}
              >
                {wordSearch!.grid.flat().map((letter, index) => (
                  <Box
                    onClick={() => tryLetter(index)}
                    key={index}
                    style={{
                      width: '24px',
                      height: '24px',
                      borderRadius: '100%',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontSize: '16px',
                      fontFamily: 'monospace',
                      cursor: 'pointer',
                      backgroundColor: lettersTried.includes(index)
                        ? 'rgb(230,100,0)'
                        : letterFound(index)
                          ? 'rgb(100,180,0)'
                          : 'unset',
                      overflow: 'hidden',
                    }}
                  >
                    {letter}
                  </Box>
                ))}
              </SimpleGrid>
            </Group>
          </Paper>
        )}
      </div>

      <Button
        onClick={open}
        style={{ position: 'fixed', right: '10px', bottom: '10px' }}
      >
        +
      </Button>

      <Modal opened={opened} onClose={close} title="Gerar novo caça-palavras">
        <form
          onSubmit={form.onSubmit((values) => {
            setWordSearchParams(values);
            handleSubmit(values);
            close();
          })}
        >
          <Stack gap="md">
            <NumberInput label="Linhas" {...form.getInputProps('rows')} />
            <NumberInput label="Colunas" {...form.getInputProps('columns')} />
            <NumberInput
              label="Quantidade de palavras"
              {...form.getInputProps('words')}
            />
            <TextInput
              label="Assunto"
              {...form.getInputProps('topic')}
            ></TextInput>
          </Stack>
          <Button mt="md" type="submit">
            Regerar
          </Button>
        </form>
      </Modal>
    </>
  );
}
