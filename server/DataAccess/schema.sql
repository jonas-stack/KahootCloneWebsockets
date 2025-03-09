-- Drop and recreate schema
DROP SCHEMA IF EXISTS kahoot CASCADE;
CREATE SCHEMA IF NOT EXISTS kahoot;

-- Enable pgcrypto for password hashing
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Create tables
CREATE TABLE kahoot.game
(
    id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name       TEXT NOT NULL,
    created_by UUID NOT NULL,
    created_at TIMESTAMPTZ DEFAULT now()
);

CREATE TABLE kahoot.player
(
    id       UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    game_id  UUID REFERENCES kahoot.game (id) ON DELETE CASCADE,
    nickname TEXT NOT NULL
);

CREATE TABLE kahoot.question
(
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    game_id       UUID REFERENCES kahoot.game (id) ON DELETE CASCADE,
    question_text TEXT NOT NULL,
    answered      BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE kahoot.question_option
(
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    question_id UUID REFERENCES kahoot.question (id) ON DELETE CASCADE,
    option_text TEXT NOT NULL,
    is_correct  BOOLEAN NOT NULL
);

CREATE TABLE kahoot.player_answer
(
    player_id          UUID REFERENCES kahoot.player (id) ON DELETE CASCADE,
    question_id        UUID REFERENCES kahoot.question (id) ON DELETE CASCADE,
    selected_option_id UUID REFERENCES kahoot.question_option (id) ON DELETE CASCADE,
    answer_timestamp   TIMESTAMP WITH TIME ZONE,
    PRIMARY KEY (player_id, question_id)
);

-- Create admin user
INSERT INTO kahoot.game (name, created_by) VALUES
    ('Movie Quiz', gen_random_uuid());

-- Admin credentials (hashed password: 'adminpassword')
INSERT INTO kahoot.player (game_id, nickname) VALUES
    ((SELECT id FROM kahoot.game LIMIT 1), 'Admin');

-- Insert players
INSERT INTO kahoot.player (game_id, nickname)
SELECT (SELECT id FROM kahoot.game LIMIT 1), 'Player' || generate_series(1,30);

-- Insert multiple questions for the movie quiz
INSERT INTO kahoot.question (game_id, question_text) VALUES
    ((SELECT id FROM kahoot.game LIMIT 1), 'Who directed the movie "Inception"?'),
    ((SELECT id FROM kahoot.game LIMIT 1), 'Which movie won Best Picture at the Oscars in 2020?'),
    ((SELECT id FROM kahoot.game LIMIT 1), 'What is the highest-grossing movie of all time?'),
    ((SELECT id FROM kahoot.game LIMIT 1), 'Which actor played Jack Dawson in "Titanic"?'),
    ((SELECT id FROM kahoot.game LIMIT 1), 'Which animated movie features a character named Buzz Lightyear?'),
    ((SELECT id FROM kahoot.game LIMIT 1), 'Who played Iron Man in the Marvel Cinematic Universe?'),
    ((SELECT id FROM kahoot.game LIMIT 1), 'What is the name of the fictional African country in "Black Panther"?'),
    ((SELECT id FROM kahoot.game LIMIT 1), 'Which film features the line "I see dead people"?'),
    ((SELECT id FROM kahoot.game LIMIT 1), 'Which movie is about a young lion prince exiled from his kingdom?'),
    ((SELECT id FROM kahoot.game LIMIT 1), 'What is the first rule of Fight Club?');

-- Insert answer options for each question
INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question WHERE question_text = 'Who directed the movie "Inception"?'),
    unnest(ARRAY['Christopher Nolan', 'Steven Spielberg', 'Quentin Tarantino', 'James Cameron']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);

INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question WHERE question_text = 'Which movie won Best Picture at the Oscars in 2020?'),
    unnest(ARRAY['Parasite', '1917', 'Joker', 'Ford v Ferrari']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);

INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question WHERE question_text = 'What is the highest-grossing movie of all time?'),
    unnest(ARRAY['Avatar', 'Avengers: Endgame', 'Titanic', 'Star Wars: The Force Awakens']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);

INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question WHERE question_text = 'Which actor played Jack Dawson in "Titanic"?'),
    unnest(ARRAY['Leonardo DiCaprio', 'Brad Pitt', 'Johnny Depp', 'Matt Damon']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);

INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question WHERE question_text = 'Which animated movie features a character named Buzz Lightyear?'),
    unnest(ARRAY['Toy Story', 'Finding Nemo', 'Shrek', 'The Incredibles']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);

INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question WHERE question_text = 'Who played Iron Man in the Marvel Cinematic Universe?'),
    unnest(ARRAY['Robert Downey Jr.', 'Chris Hemsworth', 'Mark Ruffalo', 'Chris Evans']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);

INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question WHERE question_text = 'What is the name of the fictional African country in "Black Panther"?'),
    unnest(ARRAY['Wakanda', 'Zamunda', 'Elbonia', 'Latveria']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);

INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question WHERE question_text = 'Which film features the line "I see dead people"?'),
    unnest(ARRAY['The Sixth Sense', 'The Conjuring', 'Insidious', 'Poltergeist']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);

INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question WHERE question_text = 'Which movie is about a young lion prince exiled from his kingdom?'),
    unnest(ARRAY['The Lion King', 'Madagascar', 'The Jungle Book', 'Finding Dory']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);

INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question WHERE question_text = 'What is the first rule of Fight Club?'),
    unnest(ARRAY['You do not talk about Fight Club', 'Always win', 'No rules', 'Fight every day']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);
