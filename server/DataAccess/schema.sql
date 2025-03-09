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

-- Insert questions and options for the movie quiz
INSERT INTO kahoot.question (game_id, question_text) VALUES
    ((SELECT id FROM kahoot.game LIMIT 1), 'Who directed the movie "Inception"?');

INSERT INTO kahoot.question_option (question_id, option_text, is_correct)
SELECT
    (SELECT id FROM kahoot.question LIMIT 1),
    unnest(ARRAY['Christopher Nolan', 'Steven Spielberg', 'Quentin Tarantino', 'James Cameron']),
    unnest(ARRAY[TRUE, FALSE, FALSE, FALSE]);
