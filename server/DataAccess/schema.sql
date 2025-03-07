drop schema if exists kahoot cascade;
create schema if not exists kahoot;

-- Make sure you enable the pgcrypto extension if not already done:
CREATE EXTENSION IF NOT EXISTS pgcrypto;

create table kahoot.game
(
    id         uuid primary key default gen_random_uuid(),
    name       text not null,
    created_by text not null
);

create table kahoot.question
(
    id            uuid primary key default gen_random_uuid(),
    game_id       uuid references kahoot.game (id),
    question_text text not null,
    answered      boolean not null default false
);

create table kahoot.question_option
(
    id          uuid primary key default gen_random_uuid(),
    question_id uuid references kahoot.question (id),
    option_text text not null,
    is_correct  boolean not null
);

create table kahoot.player
(
    id       uuid primary key default gen_random_uuid(),
    game_id  uuid references kahoot.game (id),
    nickname text not null,
    constraint unique_nickname_per_game unique (game_id, nickname)
);

create table kahoot.player_answer
(
    player_id          uuid references kahoot.player (id),
    question_id        uuid references kahoot.question (id),
    selected_option_id uuid references kahoot.question_option (id),
    answer_timestamp   timestamp with time zone,
    primary key (player_id, question_id)
);

create table kahoot.round_result
(
    id           uuid primary key default gen_random_uuid(),
    game_id      uuid references kahoot.game (id),
    round_number int not null,
    player_id    uuid references kahoot.player (id),
    score        int not null
);
