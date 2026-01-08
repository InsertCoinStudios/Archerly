-- WARNING: This schema is for context only and is not meant to be run.
-- Table order and constraints may not be valid for execution.

CREATE TABLE public.animal (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  species character varying NOT NULL,
  image_url text,
  CONSTRAINT animal_pkey PRIMARY KEY (id)
);
CREATE TABLE public.course_animals (
  course_id uuid NOT NULL,
  animal_id uuid NOT NULL,
  id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  order_number bigint,
  CONSTRAINT course_animals_pkey PRIMARY KEY (id),
  CONSTRAINT course_animals_animal_id_fkey FOREIGN KEY (animal_id) REFERENCES public.animal(id),
  CONSTRAINT course_animals_course_id_fkey FOREIGN KEY (course_id) REFERENCES public.courses(id)
);
CREATE TABLE public.courses (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  created_at timestamp with time zone NOT NULL DEFAULT now(),
  name text NOT NULL,
  location text,
  difficulty_id bigint,
  info text,
  CONSTRAINT courses_pkey PRIMARY KEY (id),
  CONSTRAINT courses_difficulty_id_fkey FOREIGN KEY (difficulty_id) REFERENCES public.difficulties(id)
);
CREATE TABLE public.difficulties (
  id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  difficulty character varying NOT NULL UNIQUE,
  CONSTRAINT difficulties_pkey PRIMARY KEY (id)
);
CREATE TABLE public.hunt_players (
  id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  hunt_id bigint,
  player_id bigint,
  CONSTRAINT hunt_players_pkey PRIMARY KEY (id),
  CONSTRAINT hunt_players_hunt_id_fkey FOREIGN KEY (hunt_id) REFERENCES public.hunts(id),
  CONSTRAINT hunt_players_player_id_fkey FOREIGN KEY (player_id) REFERENCES public.players(player_id)
);
CREATE TABLE public.hunts (
  id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  start_time timestamp with time zone NOT NULL DEFAULT now(),
  end_time time without time zone,
  rating_type USER-DEFINED,
  course_id uuid,
  CONSTRAINT hunts_pkey PRIMARY KEY (id),
  CONSTRAINT hunts_course_id_fkey FOREIGN KEY (course_id) REFERENCES public.courses(id)
);
CREATE TABLE public.players (
  player_id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  firstname character varying NOT NULL,
  lastname character varying,
  nickname character varying NOT NULL UNIQUE,
  user_id uuid,
  CONSTRAINT players_pkey PRIMARY KEY (player_id),
  CONSTRAINT players_user_id_fkey FOREIGN KEY (user_id) REFERENCES auth.users(id)
);
CREATE TABLE public.rounds (
  id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  course_point bigint,
  hunt_player_id bigint,
  CONSTRAINT rounds_pkey PRIMARY KEY (id),
  CONSTRAINT rounds_hunt_player_id_fkey FOREIGN KEY (hunt_player_id) REFERENCES public.hunt_players(id),
  CONSTRAINT rounds_course_point_fkey FOREIGN KEY (course_point) REFERENCES public.course_animals(id)
);
CREATE TABLE public.turn_shots (
  id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  created_at timestamp with time zone NOT NULL DEFAULT now(),
  shot_number smallint,
  round_id bigint,
  score bigint,
  CONSTRAINT turn_shots_pkey PRIMARY KEY (id),
  CONSTRAINT turn_shots_round_id_fkey FOREIGN KEY (round_id) REFERENCES public.rounds(id)
);