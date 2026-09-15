# Contributing to KrisanthiumCurrency

Thank you for your interest in contributing. This document describes the process for contributing code, documentation, tests, and reporting issues.

## Getting started

1. Fork the repository and clone your fork.
2. Create a branch for your work from `main`: `git checkout -b feature/short-descriptive-name`.

## Branch naming

- Features: `feature/<short-description>`
- Fixes: `fix/<short-description>`
- Chores: `chore/<short-description>`
- Hotfixes: `hotfix/<short-description>`

## Coding standards

- Follow the rules in `.editorconfig` included in the repository. Indentation is 4 spaces.
- Use explicit types for built-in types (avoid `var` for `int`, `string`, etc.) unless the type is obvious.
- Keep methods small and focused.

## Commit messages

Follow Conventional Commits:

- `feat: add new feature`
- `fix: fix a bug`
- `chore: update build scripts`

Provide a short summary and optionally a longer description.

## Pull requests

- Open a PR against `main` from a feature branch in your fork.
- Include a clear title, description, and linked issue if any.
- Add unit tests for new behavior and ensure existing tests pass.
- Assign at least one reviewer.

## Tests

- Add unit tests for new behavior in the `tests/` directory if present.
- All tests must pass locally before opening a PR.

## Code reviews

- Expect at least one review from a project maintainer.
- Address review comments and squash or rebase commits as requested.

## Reporting issues

- Use the repository Issues to report bugs or request features.
- Provide steps to reproduce, expected result, actual result, and environment details.

## License and CLA

By contributing, you agree that your contributions will be licensed under the project's license (MIT) and that you have the right to submit the contribution.

---

Thank you for helping improve KrisanthiumCurrency!
