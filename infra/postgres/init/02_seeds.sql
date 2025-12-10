-- 02_seeds.sql

-- Roles
INSERT INTO roles (id, name, description)
VALUES
    (uuid_generate_v4(), 'admin', 'Administrator with full permissions')
ON CONFLICT (name) DO NOTHING;

INSERT INTO roles (id, name, description)
VALUES
    (uuid_generate_v4(), 'manager', 'Manager with group-level permissions')
ON CONFLICT (name) DO NOTHING;

INSERT INTO roles (id, name, description)
VALUES
    (uuid_generate_v4(), 'member', 'Regular member with limited permissions')
ON CONFLICT (name) DO NOTHING;

-- Permissions
INSERT INTO permissions (id, key, name, description)
VALUES
    (uuid_generate_v4(), 'edit_users', 'Edit Users', 'Create, edit or delete users'),
    (uuid_generate_v4(), 'edit_tasks', 'Edit Tasks', 'Create, edit, assign and unassign tasks'),
    (uuid_generate_v4(), 'assign_users_to_groups', 'Assign Users to Groups', 'Manage user-group membership and roles'),
    (uuid_generate_v4(), 'assign_tasks_to_users', 'Assign Tasks to Users', 'Assign tasks to users'),
    (uuid_generate_v4(), 'read_reports', 'Read Reports', 'View reports and metrics')
ON CONFLICT (key) DO NOTHING;

-- Map permissions to roles
WITH r AS (
    SELECT id, name FROM roles WHERE name IN ('admin','manager','member')
), p AS (
    SELECT id, key FROM permissions
)
-- admin: all permissions
INSERT INTO permission_role (id, role_id, permission_id)
SELECT uuid_generate_v4(), r.id, p.id
FROM r
JOIN p ON TRUE
WHERE r.name = 'admin'
ON CONFLICT (role_id, permission_id) DO NOTHING;

-- manager: all except edit_users
INSERT INTO permission_role (id, role_id, permission_id)
SELECT uuid_generate_v4(), r.id, p.id
FROM r
JOIN p ON TRUE  -- unión cartesiana
WHERE r.name = 'manager' AND p.key <> 'edit_users'
ON CONFLICT (role_id, permission_id) DO NOTHING;


-- member: none -> no inserts
