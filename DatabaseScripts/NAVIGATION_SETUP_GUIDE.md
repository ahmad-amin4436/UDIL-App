# Dynamic Navigation Setup Guide

This guide explains how to implement dynamic navigation in the UDIL application.

## Overview

The navigation system has been converted from static HTML links to a dynamic database-driven system. Navigation items are now stored in the `navigation_items` table and can be managed through the database.

## Features

✅ **Dynamic Loading**: Navigation items are loaded from the database  
✅ **Role-Based Access**: Control which roles can see specific navigation items  
✅ **Grouping**: Organize navigation items into logical groups  
✅ **Sorting**: Control the order of navigation items  
✅ **Active/Inactive Toggle**: Enable/disable navigation items without deleting them  
✅ **External Links**: Support for external links that open in new tabs  

## Database Setup

### Step 1: Run the Migration Script

Execute the SQL script to create the navigation table and populate it with existing navigation items:

```bash
# Connect to your MySQL database
mysql -u your_username -p your_database_name < DatabaseScripts/AddNavigationTable.sql
```

Or run the SQL commands directly in your MySQL client.

### Step 2: Verify the Data

After running the script, verify the navigation items were created:

```sql
SELECT * FROM navigation_items ORDER BY `group`, sort_order;
```

## Database Schema

### navigation_items Table

| Column | Type | Description |
|--------|------|-------------|
| `id` | INT | Primary key, auto-increment |
| `title` | VARCHAR(100) | Display text for the navigation item |
| `url` | VARCHAR(255) | URL/path to the page (e.g., `~/Pages/DeviceCreation.aspx`) |
| `icon` | VARCHAR(50) | Bootstrap Icons class name (e.g., `house-door`, `search`) |
| `group` | VARCHAR(50) | Group name (e.g., "Write Requests", "Read Requests") |
| `sort_order` | INT | Display order within the group (lower numbers appear first) |
| `is_active` | TINYINT(1) | 1 = visible, 0 = hidden |
| `required_role` | VARCHAR(50) | User role required to see this item (NULL = visible to all) |
| `is_external` | TINYINT(1) | 1 = opens in new tab, 0 = same tab |
| `created_date` | DATETIME | Auto-set when record is created |
| `modified_date` | DATETIME | Auto-updated when record is modified |

## Managing Navigation Items

### Adding a New Navigation Item

```sql
INSERT INTO navigation_items (title, url, icon, `group`, sort_order, is_active, required_role, is_external)
VALUES (
    'New Page Title',           -- Display text
    '~/Pages/NewPage.aspx',     -- URL path
    'star',                     -- Icon name (without 'bi bi-' prefix)
    'Write Requests',           -- Group name
    15,                         -- Sort order
    1,                          -- Is active (1 = yes)
    NULL,                       -- Required role (NULL = all users)
    0                           -- Is external (0 = no)
);
```

### Updating a Navigation Item

```sql
UPDATE navigation_items 
SET 
    title = 'Updated Title',
    sort_order = 5,
    is_active = 0  -- Hide the item
WHERE id = 1;
```

### Deleting a Navigation Item

```sql
DELETE FROM navigation_items WHERE id = 1;
```

### Role-Based Access Control

To restrict a navigation item to specific roles:

```sql
-- Only visible to Admin users
UPDATE navigation_items 
SET required_role = 'Admin' 
WHERE id = 1;

-- Only visible to Operator users
UPDATE navigation_items 
SET required_role = 'Operator' 
WHERE id = 2;

-- Visible to all users (remove role restriction)
UPDATE navigation_items 
SET required_role = NULL 
WHERE id = 3;
```

### Reordering Navigation Items

To change the order of items within a group:

```sql
-- Move item to position 1
UPDATE navigation_items 
SET sort_order = 1 
WHERE id = 5;

-- Move item to position 2
UPDATE navigation_items 
SET sort_order = 2 
WHERE id = 3;
```

## Code Structure

### 1. NavigationItem Model (`UDIL.DAL/NavigationItem.cs`)
Entity class representing a navigation item with all its properties.

### 2. DatabaseLayer Methods (`UDIL.DAL/DatabaseLayer.cs`)
The following methods were added to the `DatabaseLayer` class:

- `GetAllNavigationItems()` - Get all active navigation items
- `GetGroupedNavigationItems()` - Get items grouped by their group name
- `GetNavigationItemsByGroup(string group)` - Get items for a specific group
- `AddNavigationItem(NavigationItem item)` - Add a new navigation item
- `UpdateNavigationItem(NavigationItem item)` - Update an existing navigation item
- `DeleteNavigationItem(int id)` - Delete a navigation item
- `GetNavigationItemById(int id)` - Get a single item by ID

### 3. Site.Master.cs Code-Behind
New methods added to handle navigation rendering:

- `GetGroupedNavigationItems()` - Fetches grouped navigation items from database
- `HasNavigationPermission(string requiredRole)` - Checks if user can see an item
- `GetIconClass(string icon)` - Formats icon class names
- `GetTargetAttribute(bool isExternal)` - Returns target attribute for links

### 4. Site.Master Template
The static navigation HTML has been replaced with dynamic rendering logic that:
- Fetches navigation items from the database
- Groups them by category
- Checks user permissions
- Renders the appropriate HTML

## Current Navigation Structure

The system currently has the following groups and items:

### General (No heading)
- Dashboard
- Authorization

### Write Requests
- Device Creation
- Aux Relay Operations
- Time Synchronization
- Load Control
- Load Shedding Sch.
- Update TOU
- Update IP/Port
- Meter Data Sampling
- Activate Optical Port
- Update Device Metadata
- WakeUp Sim Numbers
- Update MDI Reset
- Update Meter Status
- Update Major Alarms

### Read Requests
- DMDT On Demand Read
- AUXR On Demand Read
- DVTM On Demand Read
- SANC On Demand Read
- LSCH On Demand Read
- TIOU On Demand Read
- IPPO On Demand Read
- INST On Demand Read
- LPRO On Demand Read
- BILL On Demand Read
- MBILL On Demand Read
- OPPO On Demand Read
- WSIM On Demand Read
- MDI On Demand Read
- MTST On Demand Read
- EVNT On Demand Read
- PMAL On Demand Read

### Events Data Table
- Load Profile Data
- Instantaneous Data
- Billing Data
- Monthly Billing Data

## Available Bootstrap Icons

The system uses Bootstrap Icons. Here are some commonly used icons:

- `house-door` - Home/Dashboard
- `search` - Search/Read
- `plus-circle` - Add/Create
- `gear` - Settings
- `clock` - Time
- `calendar-x` - Calendar/Schedule
- `lightning` - Power/Energy
- `shield-lock` - Security/Authorization
- `toggle-on` - Toggle/Switch
- `clipboard-data` - Data/Reports
- `plug` - Connection/Port
- `pencil-square` - Edit/Update
- `arrow-clockwise` - Refresh/Reset
- `arrow-repeat` - Sync/Update
- `exclamation-triangle` - Warning/Alert
- `table` - Table/Data
- `reception-4` - Signal/Communication

For a complete list, visit: [Bootstrap Icons](https://icons.getbootstrap.com/)

## Troubleshooting

### Navigation items not appearing

1. Verify the database table exists:
   ```sql
   SHOW TABLES LIKE 'navigation_items';
   ```

2. Check if there are active navigation items:
   ```sql
   SELECT COUNT(*) FROM navigation_items WHERE is_active = 1;
   ```

3. Verify the connection string in `Web.config` is correct

4. Check the application logs for any database errors

### Permission issues

If navigation items are not appearing for certain users:

1. Check the user's role in the session:
   ```csharp
   string userRole = Session["Role"] as string;
   ```

2. Verify the `required_role` field in the navigation_items table

3. Remember: Users with "Admin" role can see all items regardless of `required_role`

### Active link highlighting not working

The system automatically highlights the current page. If it's not working:

1. Check that the URL in the navigation item matches the actual page URL
2. Verify the `GetActiveClass` method in `Site.Master.cs` is working correctly
3. Check browser console for JavaScript errors

## Future Enhancements

Potential improvements to consider:

1. **Admin Interface**: Create a web interface to manage navigation items without direct database access
2. **Icon Picker**: Add a visual icon picker in the admin interface
3. **Multi-language Support**: Add support for multiple languages in navigation titles
4. **Breadcrumb Generation**: Automatically generate breadcrumbs based on navigation structure
5. **Mobile Menu**: Enhanced mobile navigation with collapsible groups
6. **Analytics**: Track which navigation items are clicked most often

## Support

If you encounter any issues or have questions about this implementation, please:

1. Check this documentation first
2. Review the code comments in the source files
3. Check the database for any error messages
4. Contact the development team

---

**Last Updated**: 2026-05-14  
**Version**: 1.0.0