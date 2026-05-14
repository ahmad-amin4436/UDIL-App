-- Create tables for UDIL Session and Configuration Management
-- Execute this script in your MySQL database

-- Configurations Table
CREATE TABLE IF NOT EXISTS configurations (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    base_url VARCHAR(500) NOT NULL,
    timeout INT NOT NULL DEFAULT 60,
    db_server VARCHAR(255) NOT NULL,
    db_port VARCHAR(10) NOT NULL,
    db_name VARCHAR(255) NOT NULL,
    db_uid VARCHAR(255) NOT NULL,
    db_pwd VARCHAR(255) NOT NULL,
    db_provider VARCHAR(255) NOT NULL,
    created_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_config_name (name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Test Sessions Table
CREATE TABLE IF NOT EXISTS test_sessions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    session_id VARCHAR(32) NOT NULL UNIQUE,
    name VARCHAR(255) NOT NULL UNIQUE,
    description TEXT,
    created_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    status VARCHAR(50) NOT NULL DEFAULT 'Not Started',
    test_environment VARCHAR(100) NOT NULL DEFAULT 'Production',
    device_count INT NOT NULL DEFAULT 0,
    test_type VARCHAR(100) NOT NULL DEFAULT 'Compliance',
    tests_completed INT NOT NULL DEFAULT 0,
    total_tests INT NOT NULL DEFAULT 0,
    INDEX idx_session_name (name),
    INDEX idx_session_id (session_id),
    INDEX idx_session_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Transactions Table
CREATE TABLE IF NOT EXISTS `transactions` (
    id INT AUTO_INCREMENT PRIMARY KEY,
    transaction_id VARCHAR(100) NOT NULL,
    session_id VARCHAR(32) NULL,
    page_name VARCHAR(255) NULL,
    test_type VARCHAR(50) NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    global_device_id VARCHAR(100) NULL,
    created_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY unique_transaction_id (transaction_id),
    INDEX idx_transaction_session_id (session_id),
    INDEX idx_transaction_id (transaction_id),
    INDEX idx_transaction_status (status),
    INDEX idx_transaction_created_date (created_date),
    FOREIGN KEY (session_id) REFERENCES test_sessions(session_id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Test Results Table
CREATE TABLE IF NOT EXISTS test_results (
    id INT AUTO_INCREMENT PRIMARY KEY,
    session_id VARCHAR(32) NOT NULL,
    test_name VARCHAR(255) NOT NULL,
    test_type VARCHAR(50) NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    remarks TEXT,
    test_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    transaction_id VARCHAR(100),
    global_device_id VARCHAR(100),
    UNIQUE KEY unique_test (session_id, test_name),
    INDEX idx_session_id (session_id),
    INDEX idx_test_name (test_name),
    INDEX idx_test_status (status),
    INDEX idx_test_date (test_date),
    FOREIGN KEY (session_id) REFERENCES test_sessions(session_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Insert sample configuration data
INSERT INTO configurations (name, base_url, timeout, db_server, db_port, db_name, db_uid, db_pwd, db_provider)
VALUES ('Default', 'http://116.58.46.245:4050/UIP', 60, '116.58.46.245', '4000', 'udil33', 'accurate', 'Accurate@123', 'MySql.Data.MySqlClient')
ON DUPLICATE KEY UPDATE base_url = VALUES(base_url), timeout = VALUES(timeout);
