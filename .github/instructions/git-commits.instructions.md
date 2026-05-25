---
description: "Use when committing code, creating git commits, or finishing a task that involves git operations. Ensures only task-relevant files are committed."
applyTo: "**"
---
# Git Commit Rules

- **Never stage or commit all files** in the repository. Only commit files that were directly changed as part of the current task.
- Before committing, review the staged files and exclude anything unrelated to the task at hand.
- If unsure whether a file belongs to the current task, ask rather than including it.
- Do not commit untracked files, build artifacts, or changes from other in-progress work.
- Use `git add <specific-files>` rather than `git add .` or `git add -A`.
