-- 01_schema.sql
-- Extensiones
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ENUMS
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'user_assignment_status') THEN
        CREATE TYPE user_assignment_status AS ENUM (
            'overdue',
            'completed',
            'pending',
            'done_incomplete'
        );
    END IF;
END$$;

-- USERS
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    is_email_verified BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now()
);

-- GROUPS
CREATE TABLE IF NOT EXISTS groups (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(150) NOT NULL UNIQUE,
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now()
);

-- ROLES
CREATE TABLE IF NOT EXISTS roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now()
);

-- PERMISSIONS
CREATE TABLE IF NOT EXISTS permissions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    key VARCHAR(150) NOT NULL UNIQUE,
    name VARCHAR(150) NOT NULL,
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now()
);

-- ROLE <-> PERMISSION
CREATE TABLE IF NOT EXISTS permission_role (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    role_id UUID NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    permission_id UUID NOT NULL REFERENCES permissions(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
    CONSTRAINT uq_permission_role UNIQUE (role_id, permission_id)
);

-- ASSIGNMENTS (tasks)
CREATE TABLE IF NOT EXISTS assignments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(250) NOT NULL,
    description TEXT,
    cycles INTEGER NOT NULL DEFAULT 1 CHECK (cycles >= 1),
    periodicity INTERVAL,
    start_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
    creator_user_id UUID NOT NULL REFERENCES users(id) ON DELETE SET NULL,
    group_id UUID NOT NULL REFERENCES groups(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now()
);

-- USER_GROUP
CREATE TABLE IF NOT EXISTS user_group (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    group_id UUID NOT NULL REFERENCES groups(id) ON DELETE CASCADE,
    role_id UUID NOT NULL REFERENCES roles(id) ON DELETE RESTRICT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
    CONSTRAINT uq_user_group UNIQUE (user_id, group_id)
);

-- USER_ASSIGNMENT
CREATE TABLE IF NOT EXISTS user_assignment (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    assignment_id UUID NOT NULL REFERENCES assignments(id) ON DELETE CASCADE,
    status user_assignment_status NOT NULL DEFAULT 'pending',
    assigned_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
    CONSTRAINT uq_user_assignment UNIQUE (user_id, assignment_id)
);

-- NOTIFICATIONS
CREATE TABLE IF NOT EXISTS notifications (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    assignment_id UUID REFERENCES assignments(id) ON DELETE SET NULL,
    title VARCHAR(255) NOT NULL,
    body TEXT NOT NULL,
    sent_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
    is_sent BOOLEAN NOT NULL DEFAULT TRUE,
    type VARCHAR(50) 
);

-- REPORTS
CREATE TABLE IF NOT EXISTS reports (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE SET NULL,
    assignment_id UUID NOT NULL REFERENCES assignments(id) ON DELETE CASCADE,
    problems BOOLEAN NOT NULL DEFAULT FALSE,
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now()
);

-- INDEXES
CREATE INDEX IF NOT EXISTS idx_user_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_user_group_user ON user_group(user_id);
CREATE INDEX IF NOT EXISTS idx_user_group_group ON user_group(group_id);
CREATE INDEX IF NOT EXISTS idx_assignment_group ON assignments(group_id);
CREATE INDEX IF NOT EXISTS idx_user_assignment_user ON user_assignment(user_id);
CREATE INDEX IF NOT EXISTS idx_user_assignment_assignment ON user_assignment(assignment_id);
CREATE INDEX IF NOT EXISTS idx_reports_assignment ON reports(assignment_id);

-- TRIGGER FUNCTION updated_at
CREATE OR REPLACE FUNCTION fn_update_updated_at()
RETURNS TRIGGER AS $$
BEGIN
   NEW.updated_at = now();
   RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- TRIGGERS
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'users_updated_at_tr') THEN
        CREATE TRIGGER users_updated_at_tr
        BEFORE UPDATE ON users
        FOR EACH ROW EXECUTE FUNCTION fn_update_updated_at();
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'groups_updated_at_tr') THEN
        CREATE TRIGGER groups_updated_at_tr
        BEFORE UPDATE ON groups
        FOR EACH ROW EXECUTE FUNCTION fn_update_updated_at();
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'assignments_updated_at_tr') THEN
        CREATE TRIGGER assignments_updated_at_tr
        BEFORE UPDATE ON assignments
        FOR EACH ROW EXECUTE FUNCTION fn_update_updated_at();
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'user_assignment_updated_at_tr') THEN
        CREATE TRIGGER user_assignment_updated_at_tr
        BEFORE UPDATE ON user_assignment
        FOR EACH ROW EXECUTE FUNCTION fn_update_updated_at();
    END IF;
END$$;
