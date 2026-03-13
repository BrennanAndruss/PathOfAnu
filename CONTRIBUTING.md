# Contributing to PathOfAnu

This guide covers common Git workflows for the PathOfAnu project.

## Reverting to a Previous State of the Main Branch

There are several ways to navigate to a previous state depending on what you want to do.

### 1. View the Commit History

First, identify the commit you want to return to:

```bash
git log --oneline
```

This prints a short list of commits with their hash and message, for example:

```
abf95cb Merge pull request #1 from BrennanAndruss/expanding-scene
39d9e7a Implemented projectile casting
94992c1 Fixed shininess of demo terrain
9b0be0c Imported demo scene
```

Copy the short hash (e.g. `39d9e7a`) of the commit you want to go back to.

---

### 2. Inspect a Previous State Without Changing Anything (safe)

To temporarily look at what the project looked like at a past commit without altering `main`:

```bash
git checkout <commit-hash>
```

This puts you in a "detached HEAD" state — you can explore and build, but no changes are saved to any branch. When you're done, return to `main` with:

```bash
git checkout main
```

---

### 3. Undo Specific Commits by Adding a New Revert Commit (recommended)

`git revert` is the safest way to undo changes because it **adds** a new commit that reverses the chosen commit, keeping the full history intact:

```bash
git revert <commit-hash>
```

To revert a range of commits (reverts everything after `<last-good-hash>` up to and including `<newest-hash>`):

```bash
git revert <last-good-hash>..<newest-hash>
```

Push the revert commit to share it with the team:

```bash
git push origin main
```

---

### 4. Reset Main to a Previous Commit (destructive — use with caution)

`git reset` moves the branch pointer back in history. **This rewrites history and will cause problems for anyone who has already pulled those commits.**

Only use this if you are certain no one else has the commits you are removing:

```bash
# Move main back to a previous commit while keeping your working files intact
git reset --soft <commit-hash>

# Move main back AND discard all changes in the working directory
git reset --hard <commit-hash>
```

After a hard reset, you will need to force-push (coordinate with your team first):

```bash
git push --force origin main
```

---

### 5. Create a New Branch from a Previous Commit (recommended for experimentation)

If you want to continue development from a past point without touching `main`:

```bash
git checkout -b my-branch <commit-hash>
```

This creates and switches to a new branch starting at that commit. You can open a pull request when you're ready to merge the work back.

---

## Quick Reference

| Goal | Command |
|------|---------|
| See commit history | `git log --oneline` |
| Inspect old state (read-only) | `git checkout <hash>` then `git checkout main` |
| Undo a commit safely | `git revert <hash>` |
| Reset branch to old commit | `git reset --hard <hash>` + `git push --force` |
| Branch off an old commit | `git checkout -b <branch-name> <hash>` |
