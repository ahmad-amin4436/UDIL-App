-- Test script for database functionality
-- Execute this to verify that the database tables and basic operations work

-- Test Configuration Table
SELECT 'Testing Configuration Table...' as test_status;
DESCRIBE configurations;

-- Test Session Table  
SELECT 'Testing Session Table...' as test_status;
DESCRIBE test_sessions;

-- Test inserting a sample configuration
SELECT 'Testing Configuration Insert...' as test_status;
INSERT INTO configurations (name, base_url, timeout, db_server, db_port, db_name, db_uid, db_pwd, db_provider) 
VALUES ('Test_Config', 'http://test.com/api', 30, 'localhost', '3306', 'test_db', 'test_user', 'test_pass', 'MySql.Data.MySqlClient')
ON DUPLICATE KEY UPDATE base_url = VALUES(base_url), timeout = VALUES(timeout);

-- Test inserting a sample session
SELECT 'Testing Session Insert...' as test_status;
INSERT INTO test_sessions (session_id, name, description, status, test_environment, device_count, test_type, tests_completed, total_tests) 
VALUES ('test123', 'Test_Session', 'Test session description', 'Active', 'Development', 5, 'Integration', 2, 10)
ON DUPLICATE KEY UPDATE 
description = VALUES(description), status = VALUES(status), tests_completed = VALUES(tests_completed);

-- Test querying configurations
SELECT 'Testing Configuration Query...' as test_status;
SELECT * FROM configurations WHERE name = 'Test_Config';

-- Test querying sessions
SELECT 'Testing Session Query...' as test_status;
SELECT * FROM test_sessions WHERE name = 'Test_Session';

-- Test updating session progress
SELECT 'Testing Session Progress Update...' as test_status;
UPDATE test_sessions SET tests_completed = 5, total_tests = 10, modified_date = NOW() WHERE name = 'Test_Session';

-- Verify the update
SELECT 'Verifying Session Update...' as test_status;
SELECT name, tests_completed, total_tests, status FROM test_sessions WHERE name = 'Test_Session';

-- Clean up test data
SELECT 'Cleaning up test data...' as test_status;
DELETE FROM configurations WHERE name = 'Test_Config';
DELETE FROM test_sessions WHERE name = 'Test_Session';

SELECT 'Database functionality test completed!' as test_status;
