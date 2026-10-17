import { useEffect, useState } from 'react'
import type { SyntheticEvent } from 'react'
import {
    Plus,
    RefreshCw,
    Save,
    Trash2,
} from 'lucide-react'
import { CATEGORY_ICON_OPTIONS } from '../constants/categoryIcons'
import { getApiErrorMessage } from '../api/errorMessage'
import {
    getCategories,
    updateCategoryConfiguration,
} from '../api/categories'
import CategoryIcon from './CategoryIcon'
import type {
    VehicleCategory,
    CategoryItemRequest,
} from '../types/category'

interface CategoryFormRow {
    key: string
    id: number | null
    name: string
    icon: string
    minWeight: string
    maxWeight: string
}

function toFormRow(category: VehicleCategory): CategoryFormRow {
    return {
        key: `category-${category.id}`,
        id: category.id,
        name: category.name,
        icon: category.icon,
        minWeight: String(category.minWeight),
        maxWeight: category.maxWeight === null
            ? ''
            : String(category.maxWeight),
    }
}

function createNewRow(): CategoryFormRow {
    return {
        key: `new-${crypto.randomUUID()}`,
        id: null,
        name: '',
        icon: 'car',
        minWeight: '',
        maxWeight: '',
    }
}

function isValidDecimal(value: string): boolean {
    return /^\d+(\.\d{1,2})?$/.test(value.trim())
}

export default function CategoryManagement() {
    const [rows, setRows] = useState<CategoryFormRow[]>([])
    const [loading, setLoading] = useState(true)
    const [saving, setSaving] = useState(false)
    const [error, setError] = useState('')
    const [success, setSuccess] = useState('')

    async function loadCategories() {
        setLoading(true)
        setError('')
        setSuccess('')

        try {
            const data = await getCategories()
            setRows(data.map(toFormRow))
        } catch {
            setError('Failed to load categories.')
        } finally {
            setLoading(false)
        }
    }


    useEffect(() => {
        let cancelled = false

        async function fetchInitialCategories() {
            try {
                const data = await getCategories()

                if (!cancelled) {
                    setRows(data.map(toFormRow))
                }
            } catch {
                if (!cancelled) {
                    setError('Failed to load categories.')
                }
            } finally {
                if (!cancelled) {
                    setLoading(false)
                }
            }
        }

        void fetchInitialCategories()

        return () => {
            cancelled = true
        }
    }, [])


    function updateRow(
        key: string,
        field: 'name' | 'icon' | 'minWeight' | 'maxWeight',
        value: string,
    ) {
        setRows(current =>
            current.map(row =>
                row.key === key
                    ? { ...row, [field]: value }
                    : row,
            ),
        )

        setSuccess('')
    }

    function addRow() {
        setRows(current => [...current, createNewRow()])
        setSuccess('')
    }

    function removeRow(key: string) {
        setRows(current => current.filter(row => row.key !== key))
        setSuccess('')
    }

    function validateAndBuildRequest(): CategoryItemRequest[] {
        if (rows.length === 0) {
            throw new Error('At least one category is required.')
        }

        const items = rows.map(row => {
            const name = row.name.trim()
            const icon = row.icon.trim()
            const minText = row.minWeight.trim()
            const maxText = row.maxWeight.trim()

            if (!name || !icon) {
                throw new Error('Category name and icon are required.')
            }

            if (name.length > 100 || icon.length > 100) {
                throw new Error('Category name and icon must be at most 100 characters.')
            }

            if (!isValidDecimal(minText)) {
                throw new Error(`Invalid minimum weight for "${name}".`)
            }

            if (maxText && !isValidDecimal(maxText)) {
                throw new Error(`Invalid maximum weight for "${name}".`)
            }

            const minWeight = Number(minText)
            const maxWeight = maxText ? Number(maxText) : null

            if (maxWeight !== null && maxWeight <= minWeight) {
                throw new Error(
                    `Maximum weight must exceed minimum weight for "${name}".`,
                )
            }

            return {
                id: row.id,
                name,
                icon,
                minWeight,
                maxWeight,
            }
        })

        const names = new Set(items.map(item => item.name.toLowerCase()))

        if (names.size !== items.length) {
            throw new Error('Category names must be unique.')
        }

        const sorted = [...items].sort(
            (a, b) => a.minWeight - b.minWeight,
        )

        if (sorted[0].minWeight !== 0) {
            throw new Error('The first category must start at 0 kg.')
        }

        for (let i = 0; i < sorted.length - 1; i++) {
            const current = sorted[i]
            const next = sorted[i + 1]

            if (current.maxWeight === null) {
                throw new Error(
                    `Only the final category can have no maximum weight.`,
                )
            }

            if (current.maxWeight !== next.minWeight) {
                throw new Error(
                    `Gap or overlap between "${current.name}" and "${next.name}".`,
                )
            }
        }

        if (sorted[sorted.length - 1].maxWeight !== null) {
            throw new Error(
                'The final category must have no maximum weight.',
            )
        }

        return items
    }

    async function handleSave(
        event: SyntheticEvent<HTMLFormElement, SubmitEvent>,
    ) {
        event.preventDefault()
        setError('')
        setSuccess('')

        let categories: CategoryItemRequest[]

        try {
            categories = validateAndBuildRequest()
        } catch (err) {
            setError(
                err instanceof Error ? err.message : 'Invalid category configuration.',
            )
            return
        }

        setSaving(true)

        try {
            await updateCategoryConfiguration({ categories })
            const updated = await getCategories()
            setRows(updated.map(toFormRow))
            setSuccess('Categories saved successfully.')
        } catch (err: unknown) {
            setError(getApiErrorMessage(err))
        } finally {
            setSaving(false)
        }
    }

    return (
        <section className="vehicles-card">
            <div className="card-heading category-heading">
                <div>
                    <h2>Category Management</h2>
                    <p>
                        Configure vehicle categories and weight ranges.
                        The last category must have no maximum weight.
                    </p>
                </div>

                <button
                    type="button"
                    className="refresh-button"
                    onClick={() => void loadCategories()}
                    disabled={loading || saving}
                >
                    <RefreshCw size={16} />
                    Reload
                </button>
            </div>

            {error && (
                <div className="error-message" role="alert">
                    {error}
                </div>
            )}

            {success && (
                <div className="category-success" role="status">
                    {success}
                </div>
            )}

            {loading ? (
                <div className="table-status">
                    Loading categories...
                </div>
            ) : (
                <form onSubmit={event => void handleSave(event)}>
                    <div className="category-table-container">
                        <table className="category-table">
                            <thead>
                            <tr>
                                <th>Name</th>
                                <th>Icon</th>
                                <th>Min Weight (kg)</th>
                                <th>Max Weight (kg)</th>
                                <th>Actions</th>
                            </tr>
                            </thead>

                            <tbody>
                            {rows.map(row => (
                                <tr key={row.key}>
                                    <td>
                                        <input
                                            aria-label="Category name"
                                            value={row.name}
                                            onChange={event =>
                                                updateRow(row.key, 'name', event.target.value)
                                            }
                                            required
                                            maxLength={100}
                                            disabled={saving}
                                        />
                                    </td>

                                    <td>
                                        <div
                                            style={{
                                                display: 'flex',
                                                alignItems: 'center',
                                                gap: '10px',
                                            }}
                                        >
                                            <CategoryIcon name={row.icon} size={20} />

                                            <select
                                                aria-label="Category icon"
                                                value={row.icon}
                                                onChange={event =>
                                                    updateRow(row.key, 'icon', event.target.value)
                                                }
                                                required
                                                disabled={saving}
                                            >
                                                {row.icon && !CATEGORY_ICON_OPTIONS.some(option => option.value === row.icon) && (
                                                    <option value={row.icon}>
                                                        {row.icon} (existing icon)
                                                    </option>
                                                )}
                                                {CATEGORY_ICON_OPTIONS.map(option => (
                                                    <option key={option.value} value={option.value}>
                                                        {option.label}
                                                    </option>
                                                ))}
                                            </select>
                                        </div>
                                    </td>

                                    <td>
                                        <input
                                            aria-label="Minimum weight"
                                            type="number"
                                            min="0"
                                            step="0.01"
                                            value={row.minWeight}
                                            onChange={event =>
                                                updateRow(row.key, 'minWeight', event.target.value)
                                            }
                                            required
                                            disabled={saving}
                                        />
                                    </td>

                                    <td>
                                        <input
                                            aria-label="Maximum weight"
                                            type="number"
                                            min="0"
                                            step="0.01"
                                            placeholder="No limit"
                                            value={row.maxWeight}
                                            onChange={event =>
                                                updateRow(row.key, 'maxWeight', event.target.value)
                                            }
                                            disabled={saving}
                                        />
                                    </td>

                                    <td>
                                        <button
                                            type="button"
                                            className="category-delete"
                                            onClick={() => removeRow(row.key)}
                                            disabled={saving || rows.length <= 1}
                                            aria-label={`Remove ${row.name || 'category'}`}
                                        >
                                            <Trash2 size={17} />
                                        </button>
                                    </td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    </div>

                    <div className="category-actions">
                        <button
                            type="button"
                            className="refresh-button"
                            onClick={addRow}
                            disabled={saving}
                        >
                            <Plus size={16} />
                            Add Category
                        </button>

                        <button
                            type="submit"
                            className="primary-button"
                            disabled={saving}
                        >
                            <Save size={16} />
                            {saving ? 'Saving...' : 'Save Changes'}
                        </button>
                    </div>
                </form>
            )}
        </section>
    )
}
