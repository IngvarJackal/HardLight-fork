# Profile Recovery Runbook

Use this when a player cannot load preferences because one or more stored profile slots are malformed.

## Before You Start

1. Stop the server process.
2. Take a full database backup.
3. Determine database backend (`database.engine`): `postgres` or `sqlite`.

## Postgres

### Find user UUID by username

```sql
SELECT user_id, last_seen_user_name
FROM player
WHERE lower(last_seen_user_name) = lower('PLAYER_NAME');
```

### List profile slots for that user

```sql
SELECT pr.slot, pr.char_name
FROM profile pr
JOIN preference pf ON pf.preference_id = pr.preference_id
WHERE pf.user_id = 'USER_UUID'
ORDER BY pr.slot;
```

### Fix selected slot to a known-good slot

```sql
UPDATE preference
SET selected_character_slot = GOOD_SLOT
WHERE user_id = 'USER_UUID';
```

### Remove a bad slot (optional)

```sql
DELETE FROM profile pr
USING preference pf
WHERE pr.preference_id = pf.preference_id
  AND pf.user_id = 'USER_UUID'
  AND pr.slot = BAD_SLOT;
```

## SQLite

Open database:

```bash
sqlite3 /path/to/preferences.db
```

### Find user UUID by username

```sql
SELECT user_id, last_seen_user_name
FROM player
WHERE lower(last_seen_user_name) = lower('PLAYER_NAME');
```

### List profile slots for that user

```sql
SELECT pr.slot, pr.char_name
FROM profile pr
JOIN preference pf ON pf.preference_id = pr.preference_id
WHERE pf.user_id = 'USER_UUID'
ORDER BY pr.slot;
```

### Fix selected slot to a known-good slot

```sql
UPDATE preference
SET selected_character_slot = GOOD_SLOT
WHERE user_id = 'USER_UUID';
```

### Remove a bad slot (optional)

```sql
DELETE FROM profile
WHERE profile_id IN (
    SELECT pr.profile_id
    FROM profile pr
    JOIN preference pf ON pf.preference_id = pr.preference_id
    WHERE pf.user_id = 'USER_UUID'
      AND pr.slot = BAD_SLOT
);
```

## Validation

1. Start server.
2. Have affected player reconnect.
3. Confirm character appears and no new malformed-slot warnings are emitted.
4. Keep backup until issue is fully resolved.
