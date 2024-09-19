# Confirmed Wildcard Patterns for JobBuilderService

This document lists the confirmed and validated wildcard patterns used for including and excluding files in the `JobBuilderService`. These patterns are tailored for video and photo production workflows.

## Include Patterns

| Pattern     | Description                         |
|-------------|-------------------------------------|
| `*.mp3`     | Audio files in MP3 format           |
| `*.mp4`     | Video files in MP4 format           |
| `*.mov`     | Video files in MOV format           |
| `*.avi`     | Video files in AVI format           |
| `*.jpg`     | Image files in JPEG format          |
| `*.png`     | Image files in PNG format           |
| `*render*`  | Files containing the substring "render" |
| `*.gif`     | Image files in GIF format           |
| `*.tiff`    | Image files in TIFF format          |

## Exclude Patterns

| Pattern     | Description                         |
|-------------|-------------------------------------|
| `*temp*`    | Temporary files                     |
| `*unamed*`  | Files with "unamed" in their name    |
| `*.tmp`     | Temporary files                     |
| `backup_*`  | Backup files starting with "backup_"|
| `*.log`     | Log files                           |

## Notes

- **Inclusion Precedence:** If a file matches both an include and an exclude pattern, the **exclude pattern takes precedence**, and the file will **not** be included.
- **Wildcard Characters:**
  - `*` matches zero or more characters.
  - `?` matches exactly one character.
- **Case Sensitivity:** Matching is **case-insensitive**.

## Examples

### Included Files

- `song.mp3` (matches `*.mp3`)
- `video_render_final.mp4` (matches `*render*` and `*.mp4`)
- `photo.jpg` (matches `*.jpg`)
- `image.png` (matches `*.png`)
- `clip.avi` (matches `*.avi`)
- `animation.gif` (matches `*.gif`)
- `presentation.tiff` (matches `*.tiff`)

### Excluded Files

- `video_temp.mp4` (matches `*temp*`)
- `video_unamed_clip.mp4` (matches `*unamed*`)
- `backup_video.mp4` (matches `backup_*`)
- `error.log` (matches `*.log`)
- `notes.tmp` (matches `*.tmp`)
