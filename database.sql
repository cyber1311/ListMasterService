drop table if exists users_lists;
drop table if exists lists;
drop table if exists users_groups;
drop table if exists groups;
drop table if exists users;

create table users
(
    id       uuid not null primary key,
    email    text not null,
    name     text not null,
    password text not null
);

create table lists
(
    id       uuid not null primary key,
    title    text not null,
    is_shared bool not null,
    owner_id uuid not null references users(id),
    elements text not null
);

create table users_lists
(
    user_id uuid not null references users(id),
    list_id uuid not null references lists(id)
);

create table groups
(
    id uuid not null primary key,
    title text not null,
    owner_id uuid not null references users(id)
);

create table users_groups
(
    user_id uuid not null references users(id),
    group_id uuid not null references groups(id)
);
