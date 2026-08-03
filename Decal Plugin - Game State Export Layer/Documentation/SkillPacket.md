# Skill Packet

## Purpose

The Skill Packet is the authoritative source for character skill data.

This will replace Decal's CharacterFilter.Skills, which has proven unreliable for
certain skills (e.g. Unarmed) due to enum mismatches and COM exceptions.

---

## Known Packet Types

| Packet | Purpose | Status |
|---------|---------|--------|
| 0xF745 | Skill Update | Confirmed |
| 0xF755 | Skill Update | Confirmed |

---

## Confirmed Fields

| Packet Field | Meaning | Confidence |
|--------------|---------|------------|
| key | Skill ID | Confirmed |
| state | Training State | Confirmed |
| xp | Skill XP | Confirmed |
| raised | Skill Increment | Confirmed |
| bonus | Skill Bonus | Confirmed |
| diff | Unknown | Unknown |
| unknown2 | Unknown | Unknown |

---

## Confirmed Training States

| Value | Meaning |
|-------:|---------|
| 0 | Unusable |
| 1 | Untrained |
| 2 | Trained |
| 3 | Specialized |

---

## Confirmed Skill IDs

| ID | Skill |
|---:|-------|
| 13 | Unarmed |
| 20 | Deception |
| 38 | Alchemy |

---

## Notes

- Packet data is considered the source of truth.
- Decal CharacterFilter.Skills will eventually be removed from the skill pipeline.
- Unknown fields should remain named exactly as they appear until their purpose is proven.