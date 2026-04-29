-- Create users table for authentication
CREATE TABLE IF NOT EXISTS users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    role VARCHAR(20) NOT NULL DEFAULT 'User',
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_login DATETIME NULL,
    INDEX idx_username (username),
    INDEX idx_email (email),
    INDEX idx_active (is_active)
);

-- Insert default admin user (password: admin123 - CHANGE IN PRODUCTION)
INSERT INTO users (username, password, full_name, email, role) 
VALUES ('admin', 'admin123', 'System Administrator', 'admin@udil.com', 'Admin')
ON DUPLICATE KEY UPDATE username = username;

-- Insert default test user (password: user123)
INSERT INTO users (username, password, full_name, email, role) 
VALUES ('testuser', 'user123', 'Test User', 'test@udil.com', 'User')
ON DUPLICATE KEY UPDATE username = username;
