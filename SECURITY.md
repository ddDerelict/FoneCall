# Security Policy

We take the security and integrity of our application seriously. This document outlines our security practices, supported versions, and how to safely report a vulnerability.

## Supported Versions

Only the latest release of our application receives active security patches. If you find a vulnerability, please ensure you are testing against the newest build before reporting.

| Version | Supported          |
| ------- | ------------------ |
| Latest  | :white_check_mark: |
| < All   | :x:                |

## Reporting a Vulnerability

**Please do not open a public GitHub Issue for security vulnerabilities.** 

If you discover a security flaw, credential exposure, or potential exploit in this repository, please report it privately to ensure we can patch it before it is made public.

* **Email:** [fonecall@lifesrain.com](mailto:fonecall@lifesrain.com)
* **What to include:** A clear description of the issue, steps to reproduce, and any potential impact or proof of concept.

We will acknowledge receipt of your report within 48 hours and work with you to coordinate a timeline for a fix and public advisory.

## Our Security Commitments

To protect our users and our automated build environment, this repository adheres to the following security protocols:
* **Token Hardening:** GitHub Actions workflow tokens are strictly restricted to read-only scopes by default.
* **Immutable Dependencies:** Third-party pipeline actions are pinned to secure commit hashes to prevent supply-chain poisoning.
* **Secret Isolation:** Signing credentials are dynamically requested from Azure via OpenID Connect (OIDC) tokens and are never permanently stored in this repository.
