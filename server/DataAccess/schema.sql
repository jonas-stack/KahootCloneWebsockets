DROP SCHEMA IF EXISTS kahoot CASCADE;
CREATE SCHEMA IF NOT EXISTS kahoot;

CREATE EXTENSION IF NOT EXISTS pgcrypto;


CREATE TABLE kahoot.game
(
    id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name       TEXT NOT NULL,
    created_by UUID NOT NULL  
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

CREATE TABLE kahoot.round_result
(
    id           UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    game_id      UUID REFERENCES kahoot.game (id) ON DELETE CASCADE,
    round_number INT NOT NULL,
    player_id    UUID REFERENCES kahoot.player (id) ON DELETE CASCADE,
    score        INT NOT NULL
);
