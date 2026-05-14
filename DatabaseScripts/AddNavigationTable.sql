-- ============================================
-- Navigation Items Table Migration Script
-- Creates navigation_items table and populates
-- with existing navigation items from Site.Master
-- ============================================

-- Create navigation_items table if it doesn't exist
CREATE TABLE IF NOT EXISTS `navigation_items` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(100) NOT NULL,
  `url` VARCHAR(255) NOT NULL,
  `icon` VARCHAR(50) NULL DEFAULT NULL,
  `group` VARCHAR(50) NULL DEFAULT NULL,
  `sort_order` INT NOT NULL DEFAULT 0,
  `is_active` TINYINT(1) NOT NULL DEFAULT 1,
  `required_role` VARCHAR(50) NULL DEFAULT NULL,
  `is_external` TINYINT(1) NOT NULL DEFAULT 0,
  `created_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `modified_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_group` (`group`),
  INDEX `idx_sort_order` (`sort_order`),
  INDEX `idx_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Clear existing data (optional - comment out if you want to preserve existing data)
-- DELETE FROM navigation_items;

-- Insert General group items (top-level items without a group heading)
INSERT INTO `navigation_items` (`title`, `url`, `icon`, `group`, `sort_order`, `is_active`, `required_role`, `is_external`) VALUES
-- General/Dashboard items
('Dashboard', '~/Default.aspx', 'house-door', 'General', 1, 1, NULL, 0),
('Authorization', '~/Default.aspx?page=auth', 'shield-lock', 'General', 2, 1, NULL, 0);

-- Insert Write Requests group items
INSERT INTO `navigation_items` (`title`, `url`, `icon`, `group`, `sort_order`, `is_active`, `required_role`, `is_external`) VALUES
-- Write Requests
('Device Creation', '~/Pages/DeviceCreation.aspx', 'plus-circle', 'Write Requests', 1, 1, NULL, 0),
('Aux Relay Operations', '~/Pages/AuxRelay.aspx', 'toggle-on', 'Write Requests', 2, 1, NULL, 0),
('Time Synchronization', '~/Pages/TimeSync.aspx', 'clock', 'Write Requests', 3, 1, NULL, 0),
('Load Control', '~/Pages/LoadControl.aspx', 'lightning', 'Write Requests', 4, 1, NULL, 0),
('Load Shedding Sch.', '~/Pages/LoadSheddingScheduling.aspx', 'calendar-x', 'Write Requests', 5, 1, NULL, 0),
('Update TOU', '~/Pages/UpdateTOU.aspx', 'clock-history', 'Write Requests', 6, 1, NULL, 0),
('Update IP/Port', '~/Pages/Update_IP_Port.aspx', 'gear', 'Write Requests', 7, 1, NULL, 0),
('Meter Data Sampling', '~/Pages/MeterDataSampling.aspx', 'clipboard-data', 'Write Requests', 8, 1, NULL, 0),
('Activate Optical Port', '~/Pages/ActivateOpticalPort.aspx', 'plug', 'Write Requests', 9, 1, NULL, 0),
('Update Device Metadata', '~/Pages/UpdateDeviceMetadata.aspx', 'pencil-square', 'Write Requests', 10, 1, NULL, 0),
('WakeUp Sim Numbers', '~/Pages/WakeUpSimNumber.aspx', 'reception-4', 'Write Requests', 11, 1, NULL, 0),
('Update MDI Reset', '~/Pages/MDIReset.aspx', 'arrow-clockwise', 'Write Requests', 12, 1, NULL, 0),
('Update Meter Status', '~/Pages/UpdateMeterStatus.aspx', 'arrow-repeat', 'Write Requests', 13, 1, NULL, 0),
('Update Major Alarms', '~/Pages/UpdateMajorAlarms.aspx', 'exclamation-triangle', 'Write Requests', 14, 1, NULL, 0);

-- Insert Read Requests group items
INSERT INTO `navigation_items` (`title`, `url`, `icon`, `group`, `sort_order`, `is_active`, `required_role`, `is_external`) VALUES
-- Read Requests
('DMDT On Demand Read', '~/Read/DMDT.aspx', 'search', 'Read Requests', 1, 1, NULL, 0),
('AUXR On Demand Read', '~/Read/AUXR.aspx', 'search', 'Read Requests', 2, 1, NULL, 0),
('DVTM On Demand Read', '~/Read/DVTM.aspx', 'search', 'Read Requests', 3, 1, NULL, 0),
('SANC On Demand Read', '~/Read/SANC.aspx', 'search', 'Read Requests', 4, 1, NULL, 0),
('LSCH On Demand Read', '~/Read/LSCH.aspx', 'search', 'Read Requests', 5, 1, NULL, 0),
('TIOU On Demand Read', '~/Read/TIOU.aspx', 'search', 'Read Requests', 6, 1, NULL, 0),
('IPPO On Demand Read', '~/Read/IPPO.aspx', 'search', 'Read Requests', 7, 1, NULL, 0),
('INST On Demand Read', '~/Read/INST.aspx', 'search', 'Read Requests', 8, 1, NULL, 0),
('LPRO On Demand Read', '~/Read/LPRO.aspx', 'search', 'Read Requests', 9, 1, NULL, 0),
('BILL On Demand Read', '~/Read/BILL.aspx', 'search', 'Read Requests', 10, 1, NULL, 0),
('MBILL On Demand Read', '~/Read/MBILL.aspx', 'search', 'Read Requests', 11, 1, NULL, 0),
('OPPO On Demand Read', '~/Read/OPPO.aspx', 'search', 'Read Requests', 12, 1, NULL, 0),
('WSIM On Demand Read', '~/Read/WSIM.aspx', 'search', 'Read Requests', 13, 1, NULL, 0),
('MDI On Demand Read', '~/Read/MDI.aspx', 'search', 'Read Requests', 14, 1, NULL, 0),
('MTST On Demand Read', '~/Read/MTST.aspx', 'search', 'Read Requests', 15, 1, NULL, 0),
('EVNT On Demand Read', '~/Read/EVNT.aspx', 'search', 'Read Requests', 16, 1, NULL, 0),
('PMAL On Demand Read', '~/Read/PMAL.aspx', 'search', 'Read Requests', 17, 1, NULL, 0);

-- Insert Events Data Table group items
INSERT INTO `navigation_items` (`title`, `url`, `icon`, `group`, `sort_order`, `is_active`, `required_role`, `is_external`) VALUES
-- Events Data Tables
('Load Profile Data', '~/EventsDataTables/load_profile_data.aspx', 'table', 'Events Data Table', 1, 1, NULL, 0),
('Instantaneous Data', '~/EventsDataTables/instantaneous_data.aspx', 'table', 'Events Data Table', 2, 1, NULL, 0),
('Billing Data', '~/EventsDataTables/billing_data.aspx', 'table', 'Events Data Table', 3, 1, NULL, 0),
('Monthly Billing Data', '~/EventsDataTables/monthly_billing_data.aspx', 'table', 'Events Data Table', 4, 1, NULL, 0);

-- ============================================
-- Verification Query
-- Run this to verify the data was inserted correctly
-- ============================================
-- SELECT * FROM navigation_items ORDER BY `group`, sort_order;