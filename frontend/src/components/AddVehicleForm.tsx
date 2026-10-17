
import { useEffect, useState } from 'react'
import type { SyntheticEvent } from 'react'
import axios from 'axios'
import { X } from 'lucide-react'

import { createVehicle } from '../api/vehicles'
import { getManufacturers } from '../api/manufacturers'
import type { Manufacturer } from '../types/manufacturer'

interface AddVehicleFormProps {
    onClose: () => void
    onCreated: () => void
}

export default function AddVehicleForm({
                                           onClose,
                                           onCreated,
                                       }: AddVehicleFormProps) {
    const [manufacturers, setManufacturers] = useState<Manufacturer[]>([])

    const [ownerName, setOwnerName] = useState('')
    const [manufacturerId, setManufacturerId] = useState('')
    const [year, setYear] = useState('')
    const [weight, setWeight] = useState('')

    const [loadingManufacturers, setLoadingManufacturers] = useState(true)
    const [manufacturerError, setManufacturerError] = useState('')
    const [submitting, setSubmitting] = useState(false)
    const [error, setError] = useState('')

    useEffect(() => {
        let cancelled = false

        async function loadManufacturers() {
            try {
                const data = await getManufacturers()

                if (!cancelled) {
                    setManufacturers(data)
                }
            } catch (err) {
                console.error(err)

                if (!cancelled) {
                    setManufacturerError(
                        'Failed to load manufacturers. Please close and reopen the form.',
                    )
                }
            } finally {
                if (!cancelled) {
                    setLoadingManufacturers(false)
                }
            }
        }

        void loadManufacturers()

        return () => {
            cancelled = true
        }
    }, [])

    async function handleSubmit(event: SyntheticEvent<HTMLFormElement, SubmitEvent>) {
        event.preventDefault()
        setError('')

        const trimmedOwnerName = ownerName.trim()
        const trimmedWeight = weight.trim()
        const parsedYear = Number(year)
        const parsedWeight = Number(trimmedWeight)
        const parsedManufacturerId = Number(manufacturerId)
        const currentYear = new Date().getFullYear()

        if (!trimmedOwnerName) {
            setError('Owner name is required.')
            return
        }

        if (
            !manufacturerId ||
            !manufacturers.some(m => m.id === parsedManufacturerId)
        ) {
            setError('Please select a valid manufacturer.')
            return
        }

        if (
            !year.trim() ||
            !Number.isInteger(parsedYear) ||
            parsedYear < 1886 ||
            parsedYear > currentYear
        ) {
            setError(`Year must be between 1886 and ${currentYear}.`)
            return
        }

        if (
            !trimmedWeight ||
            !/^\d+(\.\d{1,2})?$/.test(trimmedWeight) ||
            !Number.isFinite(parsedWeight) ||
            parsedWeight <= 0 ||
            parsedWeight >= 10000000000000000
        ) {
            setError(
                'Weight must be positive and have at most 2 decimal places.',
            )
            return
        }

        setSubmitting(true)

        try {
            await createVehicle({
                ownerName: trimmedOwnerName,
                manufacturerId: parsedManufacturerId,
                yearOfManufacture: parsedYear,
                weight: parsedWeight,
            })

            onCreated()
        } catch (err) {
            console.error(err)

            if (axios.isAxiosError(err)) {
                const data: unknown = err.response?.data

                if (
                    typeof data === 'object' &&
                    data !== null &&
                    'detail' in data &&
                    typeof data.detail === 'string'
                ) {
                    setError(data.detail)
                } else {
                    setError(
                        'Unable to create vehicle. Please check your input.',
                    )
                }
            } else {
                setError('An unexpected error occurred.')
            }
        } finally {
            setSubmitting(false)
        }
    }

    return (
        <div
            className="modal-backdrop"
            onMouseDown={event => {
                if (
                    event.target === event.currentTarget &&
                    !submitting
                ) {
                    onClose()
                }
            }}
        >
            <section
                className="vehicle-modal"
                role="dialog"
                aria-modal="true"
                aria-labelledby="add-vehicle-title"
            >
                <div className="modal-heading">
                    <div>
                        <h2 id="add-vehicle-title">Add New Vehicle</h2>
                        <p>Enter the details of the vehicle.</p>
                    </div>

                    <button
                        type="button"
                        className="modal-close"
                        onClick={onClose}
                        disabled={submitting}
                        aria-label="Close form"
                    >
                        <X size={20} />
                    </button>
                </div>

                <form onSubmit={event => void handleSubmit(event)}>
                    {manufacturerError && (
                        <div className="form-error" role="alert">
                            {manufacturerError}
                        </div>
                    )}

                    {error && (
                        <div className="form-error" role="alert">
                            {error}
                        </div>
                    )}

                    <div className="form-field">
                        <label htmlFor="owner-name">Owner Name</label>

                        <input
                            id="owner-name"
                            type="text"
                            value={ownerName}
                            onChange={event => setOwnerName(event.target.value)}
                            placeholder="e.g. Alice Brown"
                            maxLength={100}
                            required
                            autoFocus
                        />
                    </div>

                    <div className="form-field">
                        <label htmlFor="manufacturer">Manufacturer</label>

                        <select
                            id="manufacturer"
                            value={manufacturerId}
                            onChange={event => setManufacturerId(event.target.value)}
                            disabled={
                                loadingManufacturers ||
                                submitting ||
                                Boolean(manufacturerError)
                            }
                            required
                        >
                            <option value="">
                                {loadingManufacturers
                                    ? 'Loading manufacturers...'
                                    : 'Select manufacturer'}
                            </option>

                            {manufacturers.map(manufacturer => (
                                <option
                                    key={manufacturer.id}
                                    value={manufacturer.id}
                                >
                                    {manufacturer.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="form-field">
                        <label htmlFor="vehicle-year">Year of Manufacture</label>

                        <input
                            id="vehicle-year"
                            type="number"
                            min={1886}
                            max={new Date().getFullYear()}
                            step={1}
                            value={year}
                            onChange={event => setYear(event.target.value)}
                            placeholder="e.g. 2021"
                            required
                        />
                    </div>

                    <div className="form-field">
                        <label htmlFor="vehicle-weight">Weight (kg)</label>

                        <input
                            id="vehicle-weight"
                            type="number"
                            min="0.01"
                            step="0.01"
                            value={weight}
                            onChange={event => setWeight(event.target.value)}
                            placeholder="e.g. 1350.50"
                            required
                        />
                    </div>

                    <div className="modal-actions">
                        <button
                            type="button"
                            className="cancel-button"
                            onClick={onClose}
                            disabled={submitting}
                        >
                            Cancel
                        </button>

                        <button
                            type="submit"
                            className="primary-button"
                            disabled={
                                submitting ||
                                loadingManufacturers ||
                                Boolean(manufacturerError)
                            }
                        >
                            {submitting ? 'Saving...' : 'Add Vehicle'}
                        </button>
                    </div>
                </form>
            </section>
        </div>
    )
}
