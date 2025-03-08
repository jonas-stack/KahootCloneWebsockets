-- Insert admin as a player
INSERT INTO kahoot.player (id, game_id, nickname)
VALUES
    ('550e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440000', 'Admin1');

-- Insert a game created by the admin
INSERT INTO kahoot.game (id, name, created_by)
VALUES
    ('550e8400-e29b-41d4-a716-446655440000', 'Quiz Night', '550e8400-e29b-41d4-a716-446655440001');

-- Insert players into the game
INSERT INTO kahoot.player (id, game_id, nickname)
VALUES
    ('600e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 'Alice'),
    ('700e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 'Bob');

-- Insert questions into the game
INSERT INTO kahoot.question (id, game_id, question_text, answered)
VALUES
    ('100e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 'What is the capital of France?', false),
    ('101e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 'What is 2 + 2?', false);

-- Insert question options
INSERT INTO kahoot.question_option (id, question_id, option_text, is_correct)
VALUES
    ('200e8400-e29b-41d4-a716-446655440000', '100e8400-e29b-41d4-a716-446655440000', 'Paris', true),
    ('300e8400-e29b-41d4-a716-446655440000', '100e8400-e29b-41d4-a716-446655440000', 'London', false),
    ('400e8400-e29b-41d4-a716-446655440000', '100e8400-e29b-41d4-a716-446655440000', 'Berlin', false),
    ('500e8400-e29b-41d4-a716-446655440000', '101e8400-e29b-41d4-a716-446655440000', '3', false),
    ('600e8400-e29b-41d4-a716-446655440000', '101e8400-e29b-41d4-a716-446655440000', '4', true),
    ('700e8400-e29b-41d4-a716-446655440000', '101e8400-e29b-41d4-a716-446655440000', '5', false);

-- Insert round results
INSERT INTO kahoot.round_result (id, game_id, round_number, player_id, score)
VALUES
    ('800e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 1, '600e8400-e29b-41d4-a716-446655440000', 10),
    ('900e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 1, '700e8400-e29b-41d4-a716-446655440000', 5);

-- Insert sample player answers
INSERT INTO kahoot.player_answer (player_id, question_id, selected_option_id, answer_timestamp)
VALUES
    ('600e8400-e29b-41d4-a716-446655440000', '100e8400-e29b-41d4-a716-446655440000', '200e8400-e29b-41d4-a716-446655440000', NOW()),
    ('700e8400-e29b-41d4-a716-446655440000', '101e8400-e29b-41d4-a716-446655440000', '600e8400-e29b-41d4-a716-446655440000', NOW());
