-- Insert the Movie Quiz Game
INSERT INTO kahoot.game (id, name, created_by)
VALUES
    ('550e8400-e29b-41d4-a716-446655440000', 'Movie Quiz', '550e8400-e29b-41d4-a716-446655440001')
ON CONFLICT (id) DO NOTHING;

-- Insert Admin (Admin doesn't answer questions)
INSERT INTO kahoot.player (id, game_id, nickname)
VALUES
    ('550e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440000', 'Admin1')
ON CONFLICT (id) DO NOTHING;

-- Insert Players
INSERT INTO kahoot.player (id, game_id, nickname)
VALUES
    ('600e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 'Alice'),
    ('700e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 'Bob')
ON CONFLICT (id) DO NOTHING;

-- Insert 2 Movie Quiz Questions
INSERT INTO kahoot.question (id, game_id, question_text, answered)
VALUES
    ('100e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 'Which movie won Best Picture at the 2020 Oscars?', false),
    ('101e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 'Who played the Joker in "The Dark Knight"?', false)
ON CONFLICT (id) DO NOTHING;

-- Insert Answer Options (4 per question, 1 correct each)
INSERT INTO kahoot.question_option (id, question_id, option_text, is_correct)
VALUES
    -- Best Picture 2020 (Correct: "Parasite")
    ('200e8400-e29b-41d4-a716-446655440000', '100e8400-e29b-41d4-a716-446655440000', 'Parasite', true),
    ('201e8400-e29b-41d4-a716-446655440000', '100e8400-e29b-41d4-a716-446655440000', 'Joker', false),
    ('202e8400-e29b-41d4-a716-446655440000', '100e8400-e29b-41d4-a716-446655440000', '1917', false),
    ('203e8400-e29b-41d4-a716-446655440000', '100e8400-e29b-41d4-a716-446655440000', 'Once Upon a Time in Hollywood', false)
ON CONFLICT (id) DO NOTHING;

INSERT INTO kahoot.question_option (id, question_id, option_text, is_correct)
VALUES
    -- The Joker Actor (Correct: "Heath Ledger")
    ('204e8400-e29b-41d4-a716-446655440000', '101e8400-e29b-41d4-a716-446655440000', 'Joaquin Phoenix', false),
    ('205e8400-e29b-41d4-a716-446655440000', '101e8400-e29b-41d4-a716-446655440000', 'Heath Ledger', true),
    ('206e8400-e29b-41d4-a716-446655440000', '101e8400-e29b-41d4-a716-446655440000', 'Jack Nicholson', false),
    ('207e8400-e29b-41d4-a716-446655440000', '101e8400-e29b-41d4-a716-446655440000', 'Jared Leto', false)
ON CONFLICT (id) DO NOTHING;

-- Insert round results
INSERT INTO kahoot.round_result (id, game_id, round_number, player_id, score)
VALUES
    ('800e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 1, '600e8400-e29b-41d4-a716-446655440000', 10),
    ('900e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440000', 1, '700e8400-e29b-41d4-a716-446655440000', 5)
ON CONFLICT (id) DO NOTHING;

-- Insert sample player answers (One per player)
INSERT INTO kahoot.player_answer (player_id, question_id, selected_option_id, answer_timestamp)
VALUES
    ('600e8400-e29b-41d4-a716-446655440000', '100e8400-e29b-41d4-a716-446655440000', '200e8400-e29b-41d4-a716-446655440000', NOW()), -- Alice chose "Parasite"
    ('700e8400-e29b-41d4-a716-446655440000', '101e8400-e29b-41d4-a716-446655440000', '205e8400-e29b-41d4-a716-446655440000', NOW()) -- Bob chose "Heath Ledger"
ON CONFLICT (player_id, question_id) DO NOTHING;
